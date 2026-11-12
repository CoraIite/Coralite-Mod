using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.ShadowSpike, typeof(ShadowBallContext))]
    public sealed class ShadowBallShadowSpikeState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>短暂前摇，算出玩家下方的落点</summary>
            Ready,
            /// <summary>引力移动</summary>
            GravityMove,
            /// <summary>等小球一字排开</summary>
            WaitBalls,
            /// <summary>临时阶段，在结束之前</summary>
            preOver,
            /// <summary>上戳后摇</summary>
            Over,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.ShadowSpike;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Ready:
                    ctx.SetVelocitySlowDown(0.95f);
                    ctx.SetRotationLetItGo();
                    ctx.Boss.ShellBreath_HideCore(2);

                    if (Timer > 20)
                    {
                        Player target = ctx.Target;
                        Vector2 aimPos = target.Center;
                        float distance = Vector2.Distance(ctx.Npc.Center, target.Center);

                        if (target.velocity.LengthSquared() > 1)
                        {
                            aimPos += target.velocity.SafeNormalize(Vector2.Zero)
                                * Helper.Clamp(distance / 600f, 0.2f, 1f) * 300f;
                        }

                        aimPos += (target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero)
                            * Helper.Clamp(distance / 600f, 0.35f, 1f) * 400f;

                        ctx.GravityAnchor = new Vector2(aimPos.X, target.Center.Y + 350);
                        ctx.GravityMoveReady(ctx.GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.GravityMove);
                    }
                    break;
                case Beat.GravityMove:
                    if (ctx.GravityMove(ctx.GravityAnchor, Timer))
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                        SwitchBeat(ctx, (int)Beat.WaitBalls);
                    }
                    break;
                case Beat.WaitBalls:
                    ctx.SetVelocityLetItGo();
                    ctx.SetRotationLerp(0f, 0.2f);
                    float f = MathF.Sin(Timer * 0.05f);//横向的摆锤运动
                    ctx.CoreOffsetAngle = f > 0 ? 0 : MathHelper.Pi;
                    ctx.CoreOffsetRadius = MathF.Sign(f) * 40;
                    ctx.CoreOffset_AngledTo(0.15f);
                    ctx.Boss.CommonShellBreath(2);

                    break;
                case Beat.preOver:
                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                    SwitchBeat(ctx, (int)Beat.Over);
                    break;
                case Beat.Over:
                    if (Timer == 1)
                    {
                        ctx.Npc.velocity.Y = -16f;

                        float dis = MathF.Abs(ctx.Target.Center.X - ctx.Npc.Center.X);
                        if (dis > 16 * 30)
                            ctx.Npc.velocity.X += MathF.Sign(ctx.Target.Center.X - ctx.Npc.Center.X) * Helper.Clamp(dis / 800f, 0.2f, 1f) * 10f;

                        ctx.NetUpdate();
                    }

                    ctx.SetVelocitySlowDown(0.97f);
                    ctx.SetRotationLerp((ctx.Target.Center-ctx.Npc.Center).ToRotation(),0.15f);
                    ctx.CoreOffset_BackToCenter(0.2f);
                    ctx.Boss.ShellBreath_ShowCore(2);

                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.WaitBalls:
                    if (Timer == 0)
                    {
                        DeploySmallBalls(ctx);
                    }

                    if (!ctx.Boss.CheckSmallBallReady())
                    {
                        return null;
                    }

                    SwitchBeat(ctx, (int)Beat.preOver);
                    return null;

                case Beat.Over:
                    if (Timer > 80)
                        //return EndAttack(ctx);
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                    return null;
                default:
                    return null;
            }
        }

        private static IVaultState<ShadowBallContext> DeploySmallBalls(ShadowBallContext ctx)
        {
            ShadowBall boss = ctx.Boss;
            int smallBallCount = boss.GetSmallBalls();

            if (smallBallCount < 1)
            {
                return EndAttack(ctx);
            }

            int howMany = CallBackCount(ctx, smallBallCount, 16*7);
            boss.CommandSmallBalls(SmallShadowBallStateId.ShadowSpike, howMany, readyRest: true);
            ctx.NetUpdate();
            return null;
        }
    }
}
