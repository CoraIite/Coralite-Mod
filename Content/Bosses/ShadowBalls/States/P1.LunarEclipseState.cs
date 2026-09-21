using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 月食：本体绕着玩家换位，每换一次就派一颗小球贴到身前再射出去，沿途留下月相弹幕。<br/>
    /// 旧 <c>ShadowBall.LunarEclipse</c>（P1.LunarEclipse.cs:9-144）；旧收尾是 <c>SwitchState_Test(OnSpawnAnmi)</c> 的调试死循环，已换回 hub。
    /// </summary>
    [VaultState((int)ShadowBallStateId.LunarEclipse, typeof(ShadowBallContext))]
    public sealed class ShadowBallLunarEclipse : ShadowBallStateBase
    {
        /// <summary>月食的子拍。旧 <c>SonState</c> 0~4。</summary>
        private enum Beat
        {
            /// <summary>起手圆环特效。</summary>
            _0_StartEffect,
            /// <summary>被裂隙牵引到玩家周围的换位点。</summary>
            _1_GravityMove,
            /// <summary>到位后持续朝向玩家。</summary>
            _2_FacePlayer,
            /// <summary>数一轮，够了就收招，不够就换个角度再来。</summary>
            _3_CheckLoop,
            /// <summary>收招后摇。</summary>
            _4_End,
        }

        /// <summary>换位点的极角（旧 <c>Recorder2</c>）。落点由它算出，客户端也要用，进热槽 C。</summary>
        private float orbitAngle;

        /// <summary>已经换了几次位（旧 <c>Recorder3</c> = <c>localAI[0]</c>）。进热槽 D。</summary>
        private int loopCount;

        public override ShadowBallStateId StateIndex => ShadowBallStateId.LunarEclipse;

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);

            // 旧 Recorder3 借的是 localAI[0]，而入态只清 Recorder / Recorder2，于是循环数会跨招累积
            // （旧代码全靠调试用的 SwitchState_Test 顺手清零掩盖了这一点）。改成状态自有字段后天然一招一清。
            orbitAngle = 0f;
            loopCount = 0;
        }

        public override void WriteHot(ShadowBallContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[BossSlots.C] = orbitAngle;
            ctx.Hot[BossSlots.D] = loopCount;
        }

        public override void ReadHot(ShadowBallContext ctx)
        {
            base.ReadHot(ctx);
            orbitAngle = ctx.Hot[BossSlots.C];
            loopCount = (int)ctx.Hot[BossSlots.D];
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 起手圆环特效的持续时长，到点后开始第一次引力移动。沿用旧值 P1.LunarEclipse.cs:26。
            const int EclipseStartFrames = 45;

            // 每次引力移动的落点半径（以玩家为圆心）。沿用旧值 P1.LunarEclipse.cs:32,107。
            const float EclipseOrbitRadius = 450f;

            // 每轮之间落点角度的推进量。沿用旧值 P1.LunarEclipse.cs:104（旧代码把随机区间注释掉了，固定 2π/3）。
            const float EclipseAngleStep = MathHelper.TwoPi / 3f;

            // 到位后持续朝向玩家的插值爬坡时长与该段总时长。沿用旧值 P1.LunarEclipse.cs:81,86。
            const float EclipseFaceRampFrames = 50f;
            const int EclipseFaceFrames = 100;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat._0_StartEffect:
                    // TODO（作者原注）：起手的白色圆环特效 Particle 还没做。
                    ctx.DeclareKeep();
                    if (Timer >= EclipseStartFrames)
                    {
                        // 角度掷骰改走已同步的 AttackSeed：旧代码用 Main.rand 且跑在两端，客户端必然掷出另一个角度。
                        orbitAngle = (float)(ctx.CreateAttackRandom().NextDouble() * MathHelper.TwoPi);

                        // 沿用旧值 P1.LunarEclipse.cs:32：那一行读的是 Recorder（入态已清零）而不是刚掷出的角度，
                        // 所以第一次换位恒定落在玩家正右侧。设计文档 §月食 写的是"根据 recorder 记录角度"，
                        // 与代码冲突；按 D10 原样保留，差异记进报告等作者定。
                        GravityAnchor = ctx.Target.Center + (Vector2.UnitX * EclipseOrbitRadius);
                        ctx.GravityMoveReady(GravityAnchor);
                        SwitchBeat(ctx, (int)Beat._1_GravityMove);
                    }

                    break;

                case Beat._1_GravityMove:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        // 直接赋值而不是 SwitchLockState：旧 P1.LunarEclipse.cs:71 就没重置 LockLerpPercent，锁环是接着上一次的进度过渡的。
                        ctx.Boss.LockState = ShadowBall.LockStates.ConcentricCirclesAngled;
                        SwitchBeat(ctx, (int)Beat._2_FacePlayer);
                    }

                    break;

                case Beat._2_FacePlayer:
                    ctx.DeclareKeep();
                    ctx.DeclareRotation((ctx.Target.Center - ctx.Npc.Center).ToRotation(),
                        Helper.Clamp(Timer / EclipseFaceRampFrames, 0, 1));

                    if (Timer >= EclipseFaceFrames)
                    {
                        SwitchBeat(ctx, (int)Beat._3_CheckLoop);
                    }

                    break;

                case Beat._3_CheckLoop:
                    ctx.DeclareKeep();
                    loopCount++;

                    if (loopCount < ShadowBallDirector.EclipseLoopCount())
                    {
                        orbitAngle += EclipseAngleStep;
                        GravityAnchor = ctx.Target.Center + (orbitAngle.ToRotationVector2() * EclipseOrbitRadius);
                        ctx.GravityMoveReady(GravityAnchor);
                        SwitchBeat(ctx, (int)Beat._1_GravityMove);
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat._4_End);
                    }

                    break;

                case Beat._4_End:
                    ctx.DeclareKeep();
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 收招后摇时长。沿用旧值 P1.LunarEclipse.cs:135。
            const int EclipseEndFrames = 30;

            switch ((Beat)BeatIndex)
            {
                // Timer == 0 = 本帧 SharedUpdate 刚换到这一拍，跨实体编排只在权威端发生一次。
                case Beat._2_FacePlayer when Timer == 0:
                    ctx.Boss.CommandOneIdleSmallBall(SmallShadowBallStateId.LunarEclipse);
                    ctx.MarkDecision();
                    return null;

                case Beat._4_End when Timer == 0:
                    ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Idle, int.MaxValue);
                    ctx.MarkDecision();
                    return null;

                case Beat._4_End:
                    if (Timer >= EclipseEndFrames)
                    {
                        //return EndAttack(ctx);
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.OnSpawnAnim);

                    }

                    return null;

                default:
                    return null;
            }
        }
    }
}
