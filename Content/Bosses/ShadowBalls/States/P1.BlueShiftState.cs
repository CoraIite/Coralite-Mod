using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.BlueShift, typeof(ShadowBallContext))]
    public sealed class ShadowBallBlueShiftState : ShadowBallStateBase
    {
        private enum Beat
        {
            Ready,
            Gather,
            Over,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.BlueShift;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocitySlowDown(0.95f);

            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    if (Timer==2)
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);
                    }

                    float f = Timer / 60f;

                    ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.15f);
                    ctx.Boss.LockDistancePercent = Helper.Lerp(1, 2, Helper.BezierEase(f));
                    ctx.CoreOffsetAngle = ctx.CoreOffsetAngle.AngleLerp(ctx.Npc.rotation, 0.2f);
                    ctx.CoreOffsetRadius = Helper.SqrtEase(f) * 40;
                    ctx.CoreOffset_AngledTo(0.2f);

                    if (Timer >= 60)
                        SwitchBeat(ctx, (int)Beat.Gather);
                    break;
                case Beat.Gather://牵引玩家
                    if (Timer < 15)
                    {
                        ctx.Boss.LockDistancePercent = Helper.Lerp(2, 0.6f, Helper.SqrtEase(Timer / 15f));
                        ctx.CoreOffset_BackToCenter(0.2f);

                        const float pullDistance = 16 * 80;

                        if (!VaultUtils.isServer && Vector2.DistanceSquared(Main.LocalPlayer.Center, ctx.Npc.Center) <= pullDistance * pullDistance)
                        {
                            Main.LocalPlayer.velocity += (ctx.Npc.Center - Main.LocalPlayer.Center).SafeNormalize(Vector2.Zero) * Helper.Clamp((ctx.Npc.Center - Main.LocalPlayer.Center).Length() / 600 * 1.4f, 0.6f, 1.4f);
                        }
                    }
                    else
                    {
                        ctx.CoreOffset_BackToCenter(0.1f);

                        ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 1, 0.03f);
                    }

                    if (Timer > 80)
                    {
                        ctx.Boss.LockDistancePercent = 1;
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);

                        SwitchBeat(ctx, (int)Beat.Over);
                    }
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备

                    break;
                case Beat.Gather:
                    if (Timer == 0)
                    {
                        Color color = Color.CornflowerBlue*0.7f;
                        ctx.Npc.NewProjectileDirectInAI_Server<AbsorbProj>(ctx.Npc.Center, Vector2.Zero, 0, 0, ai0: color.R, ai1: color.G, ai2: color.B);

                        float dir = (ctx.Target.Center - ctx.Npc.Center).ToRotation();
                        const float length = 16 * 100;
                        const float speed = length / (30 + 60 + 30);

                        for (int i = 0; i < 12; i++)
                        {
                            Vector2 direction = dir.ToRotationVector2();

                            ctx.Npc.NewProjectileDirectInAI_Server<ShiftStar>(ctx.Npc.Center + direction * length, -direction * speed, Helper.GetProjDamage(60, 80, 120), 0, ai0: 0);

                            dir += MathHelper.TwoPi / 12;
                        }
                    }
                    break;
                case Beat.Over:
                    if (Timer >= 30)
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                        //return EndAttack(ctx);
                    break;
            }

            return null;
        }
    }
}
