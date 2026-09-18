using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 影之公转：引力移动到玩家前方，把小球召回身边排成几个环，随后放出紫光并持续抛出影子弹幕。<br/>
    /// 旧 <c>ShadowBall.Revolution</c>（P1.Revolution.cs:9-127）。这一招的收尾原本就是 <c>SwitchP1State()</c>，没被调试改过。
    /// </summary>
    [VaultState((int)ShadowBallStateId.Revolution, typeof(ShadowBallContext))]
    public sealed class ShadowBallRevolutionState : ShadowBallStateBase
    {
        /// <summary>公转的子拍。旧 <c>SonState</c> Ready / Move / CallBackSmallBall / ShootLight / LightBack。</summary>
        private enum Beat
        {
            /// <summary>短暂前摇，算出落点。</summary>
            Ready,
            /// <summary>被裂隙牵引到落点。</summary>
            Move,
            /// <summary>小球归位。</summary>
            CallBack,
            /// <summary>放光并持续抛影子弹幕。</summary>
            ShootLight,
            /// <summary>收光后摇。</summary>
            LightBack,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.Revolution;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Ready:
                    ctx.DeclareKeep();
                    if (Timer > ShadowBallDirector.RevolutionReadyFrames)
                    {
                        // 落点两端各自算（读的全是原版同步量），随后再随热槽 A/B 过线给中途加入者。
                        GravityAnchor = PredictTarget(ctx, ShadowBallDirector.RevolutionLeadLength, ShadowBallDirector.RevolutionOvershootLength);
                        ctx.GravityMoveReady(GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.Move);
                    }

                    break;

                case Beat.Move:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                        SwitchBeat(ctx, (int)Beat.CallBack);
                    }

                    break;

                case Beat.CallBack:
                    ctx.DeclareKeep();
                    if (Timer > ShadowBallDirector.RevolutionGatherFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.ShootLight);
                    }

                    break;

                case Beat.ShootLight:
                    ctx.DeclareApproach(ctx.Target.Center, ShadowBallDirector.RevolutionKeepDistance,
                        ShadowBallDirector.RevolutionApproachSpeed, ShadowBallDirector.RevolutionApproachLerp,
                        ShadowBallDirector.RevolutionHoldDamp);

                    if (Timer > ShadowBallDirector.RevolutionShootFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.LightBack);
                    }

                    break;

                case Beat.LightBack:
                    ctx.DeclareDamp(ShadowBallDirector.RevolutionEndDamp);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                // Timer == 0 只可能出现在"本帧 SharedUpdate 刚换过拍"之后（基座在 OnUpdate 开头先 Timer++），
                // 用它当"刚到位"的一次性拍，跨实体编排就只在权威端发生一次。
                case Beat.CallBack when Timer == 0:
                    return CallBackSmallBalls(ctx);

                case Beat.ShootLight:
                    if (Timer % ShadowBallDirector.RevolutionShadowInterval == 0)
                    {
                        ctx.Npc.NewProjectileDirectInAI_Server<ShadowBallOrbitShadow>(ctx.Npc.Center,
                            Main.rand.NextVector2CircularEdge(1, 1) * ShadowBallDirector.RevolutionShadowSpeed,
                            ShadowBallDirector.RevolutionShadowDamage(), 0, ai0: ShadowBallDirector.RevolutionShadowLife);
                        ctx.MarkDecision();
                    }

                    return null;

                case Beat.LightBack:
                    if (Timer > ShadowBallDirector.RevolutionEndFrames)
                    {
                        return EndAttack(ctx);
                    }

                    return null;

                default:
                    return null;
            }
        }

        /// <summary>
        /// 到位后把小球叫回来排环。旧 P1.Revolution.cs:64-86：一个都没有就直接收招，
        /// 否则按与玩家的 X 距离决定叫几个，被叫到的切到小球的公转态。
        /// </summary>
        private static IVaultState<ShadowBallContext> CallBackSmallBalls(ShadowBallContext ctx)
        {
            ShadowBall boss = ctx.Boss;
            int smallBallCount = boss.GetSmallBalls();

            if (smallBallCount < 1)
            {
                return EndAttack(ctx);
            }

            int howMany = CallBackCount(ctx, smallBallCount, ShadowBallDirector.RevolutionPerLength);
            boss.CommandSmallBalls(SmallShadowBallStateId.Revolution, howMany);
            ctx.MarkDecision();
            return null;
        }
    }
}
