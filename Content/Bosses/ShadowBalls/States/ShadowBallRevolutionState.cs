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
            // 起手前摇。沿用旧值 P1.Revolution.cs:36。
            const int RevolutionReadyFrames = 10;

            // 沿玩家速度方向的预判量与其距离归一化分母、下限。沿用旧值 P1.Revolution.cs:42-43。
            const float RevolutionLeadLength = 120f;

            // 沿"自身→玩家"方向的越位量与其距离归一化分母。沿用旧值 P1.Revolution.cs:46。
            const float RevolutionOvershootLength = 200f;

            // 小球归位的等待时长。沿用旧值 P1.Revolution.cs:91。
            const int RevolutionGatherFrames = 90;

            // 放光段：离玩家超过这个距离就缓慢靠近，否则原地衰减。沿用旧值 P1.Revolution.cs:100。
            const float RevolutionKeepDistance = 600f;
            const float RevolutionApproachSpeed = 4f;
            const float RevolutionApproachLerp = 0.04f;
            const float RevolutionHoldDamp = 0.94f;

            // 放光段总时长。沿用旧值 P1.Revolution.cs:111。
            const int RevolutionShootFrames = 180;

            // 收光后摇的时长与速度衰减。沿用旧值 P1.Revolution.cs:120,121。
            const float RevolutionEndDamp = 0.9f;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Ready:
                    ctx.DeclareKeep();
                    if (Timer > RevolutionReadyFrames)
                    {
                        // 落点两端各自算（读的全是原版同步量），随后再随热槽 A/B 过线给中途加入者。
                        GravityAnchor = PredictTarget(ctx, RevolutionLeadLength, RevolutionOvershootLength);
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
                    if (Timer > RevolutionGatherFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.ShootLight);
                    }

                    break;

                case Beat.ShootLight:
                    ctx.DeclareApproach(ctx.Target.Center, RevolutionKeepDistance,
                        RevolutionApproachSpeed, RevolutionApproachLerp,
                        RevolutionHoldDamp);

                    if (Timer > RevolutionShootFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.LightBack);
                    }

                    break;

                case Beat.LightBack:
                    ctx.DeclareDamp(RevolutionEndDamp);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 放光段每隔多少帧抛一颗影子公转弹幕，以及它的初速与存活参数 ai0。沿用旧值 P1.Revolution.cs:105,109。
            const int RevolutionShadowInterval = 24;
            const float RevolutionShadowSpeed = 6f;
            const float RevolutionShadowLife = 90f;

            // 收光后摇的时长与速度衰减。沿用旧值 P1.Revolution.cs:120,121。
            const int RevolutionEndFrames = 45;

            switch ((Beat)BeatIndex)
            {
                // Timer == 0 只可能出现在"本帧 SharedUpdate 刚换过拍"之后（基座在 OnUpdate 开头先 Timer++），
                // 用它当"刚到位"的一次性拍，跨实体编排就只在权威端发生一次。
                case Beat.CallBack when Timer == 0:
                    return CallBackSmallBalls(ctx);

                case Beat.ShootLight:
                    if (Timer % RevolutionShadowInterval == 0)
                    {
                        ctx.Npc.NewProjectileDirectInAI_Server<ShadowBallOrbitShadow>(ctx.Npc.Center,
                            Main.rand.NextVector2CircularEdge(1, 1) * RevolutionShadowSpeed,
                            ShadowBallDirector.RevolutionShadowDamage(), 0, ai0: RevolutionShadowLife);
                        ctx.MarkDecision();
                    }

                    return null;

                case Beat.LightBack:
                    if (Timer > RevolutionEndFrames)
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
            // 召回小球的身位换算：与玩家的 X 距离每 RevolutionPerLength 像素多叫一个，再加基数。沿用旧值 P1.Revolution.cs:76,78。
            const float RevolutionPerLength = 16 * 7;

            ShadowBall boss = ctx.Boss;
            int smallBallCount = boss.GetSmallBalls();

            if (smallBallCount < 1)
            {
                return EndAttack(ctx);
            }

            int howMany = CallBackCount(ctx, smallBallCount, RevolutionPerLength);
            boss.CommandSmallBalls(SmallShadowBallStateId.Revolution, howMany);
            ctx.MarkDecision();
            return null;
        }
    }
}
