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
    public sealed class ShadowBallLunarEclipseState : ShadowBallStateBase
    {
        /// <summary>月食的子拍。旧 <c>SonState</c> 0~4。</summary>
        private enum Beat
        {
            /// <summary>起手圆环特效。</summary>
            StartEffect,
            /// <summary>被裂隙牵引到玩家周围的换位点。</summary>
            GravityMove,
            /// <summary>到位后持续朝向玩家。</summary>
            FacePlayer,
            /// <summary>数一轮，够了就收招，不够就换个角度再来。</summary>
            CheckLoop,
            /// <summary>收招后摇。</summary>
            End,
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
            ctx.Hot[CoraliteBossHotSlots.C] = orbitAngle;
            ctx.Hot[CoraliteBossHotSlots.D] = loopCount;
        }

        public override void ReadHot(ShadowBallContext ctx)
        {
            base.ReadHot(ctx);
            orbitAngle = ctx.Hot[CoraliteBossHotSlots.C];
            loopCount = (int)ctx.Hot[CoraliteBossHotSlots.D];
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.StartEffect:
                    // TODO（作者原注）：起手的白色圆环特效 Particle 还没做。
                    ctx.DeclareKeep();
                    if (Timer >= ShadowBallDirector.EclipseStartFrames)
                    {
                        // 角度掷骰改走已同步的 AttackSeed：旧代码用 Main.rand 且跑在两端，客户端必然掷出另一个角度。
                        orbitAngle = (float)(ctx.CreateAttackRandom().NextDouble() * MathHelper.TwoPi);

                        // 沿用旧值 P1.LunarEclipse.cs:32：那一行读的是 Recorder（入态已清零）而不是刚掷出的角度，
                        // 所以第一次换位恒定落在玩家正右侧。设计文档 §月食 写的是"根据 recorder 记录角度"，
                        // 与代码冲突；按 D10 原样保留，差异记进报告等作者定。
                        GravityAnchor = ctx.Target.Center + (Vector2.UnitX * ShadowBallDirector.EclipseOrbitRadius);
                        ctx.GravityMoveReady(GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.GravityMove);
                    }

                    break;

                case Beat.GravityMove:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        // 直接赋值而不是 SwitchLockState：旧 P1.LunarEclipse.cs:71 就没重置 LockLerpPercent，锁环是接着上一次的进度过渡的。
                        ctx.Boss.LockState = ShadowBall.LockStates.ConcentricCirclesAngled;
                        SwitchBeat(ctx, (int)Beat.FacePlayer);
                    }

                    break;

                case Beat.FacePlayer:
                    ctx.DeclareKeep();
                    ctx.DeclareRotation((ctx.Target.Center - ctx.Npc.Center).ToRotation(),
                        Helper.Clamp(Timer / ShadowBallDirector.EclipseFaceRampFrames, 0, 1));

                    if (Timer >= ShadowBallDirector.EclipseFaceFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.CheckLoop);
                    }

                    break;

                case Beat.CheckLoop:
                    ctx.DeclareKeep();
                    loopCount++;

                    if (loopCount < ShadowBallDirector.EclipseLoopCount())
                    {
                        orbitAngle += ShadowBallDirector.EclipseAngleStep;
                        GravityAnchor = ctx.Target.Center + (orbitAngle.ToRotationVector2() * ShadowBallDirector.EclipseOrbitRadius);
                        ctx.GravityMoveReady(GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.GravityMove);
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat.End);
                    }

                    break;

                case Beat.End:
                    ctx.DeclareKeep();
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                // Timer == 0 = 本帧 SharedUpdate 刚换到这一拍，跨实体编排只在权威端发生一次。
                case Beat.FacePlayer when Timer == 0:
                    ctx.Boss.CommandOneIdleSmallBall(SmallShadowBallStateId.LunarEclipse);
                    ctx.MarkDecision();
                    return null;

                case Beat.End when Timer == 0:
                    ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Idle, int.MaxValue);
                    ctx.MarkDecision();
                    return null;

                case Beat.End:
                    if (Timer >= ShadowBallDirector.EclipseEndFrames)
                    {
                        return EndAttack(ctx);
                    }

                    return null;

                default:
                    return null;
            }
        }
    }
}
