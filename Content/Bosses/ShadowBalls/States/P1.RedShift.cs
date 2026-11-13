using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Bosses.VanillaReinforce.EoC;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.RedShift, typeof(ShadowBallContext))]
    public sealed class ShadowBallRedShiftState : ShadowBallStateBase
    {
        private enum Beat
        {
            Ready,
            Push,
            Over,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.RedShift;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocitySlowDown(0.95f);

            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    if (Timer == 2)
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);
                    }

                    float f = Timer / 60f;

                    ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.15f);
                    ctx.Boss.LockDistancePercent = Helper.Lerp(1, 0.6f, Helper.BezierEase(f));
                    ctx.CoreOffsetAngle = ctx.CoreOffsetAngle.AngleLerp(ctx.Npc.rotation, 0.2f);
                    ctx.CoreOffsetRadius = Helper.SqrtEase(f) * 40;
                    ctx.CoreOffset_AngledTo(0.2f);

                    if (Timer >= 60)
                        SwitchBeat(ctx, (int)Beat.Push);
                    break;
                case Beat.Push://推开玩家
                    if (Timer < 15)
                    {
                        ctx.Boss.LockDistancePercent = Helper.Lerp(0.6f, 2f, Helper.SqrtEase(Timer / 15f));
                        ctx.CoreOffset_BackToCenter(0.2f);

                        const float pullDistance = 16 * 80;

                        if (!VaultUtils.isServer && Vector2.DistanceSquared(Main.LocalPlayer.Center, ctx.Npc.Center) <= pullDistance * pullDistance)
                        {
                            Main.LocalPlayer.velocity += (Main.LocalPlayer.Center-ctx.Npc.Center).SafeNormalize(Vector2.Zero) * Helper.Clamp(400 / (ctx.Npc.Center - Main.LocalPlayer.Center).Length() * 1.7f, 0.6f, 1.4f);
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
                case Beat.Push:
                    if (Timer == 0)
                    {
                        Color color = Color.PaleVioletRed * 0.7f;
                        ctx.Npc.NewProjectileDirectInAI_Server<FightingRoar>(ctx.Npc.Center, Vector2.Zero, 0, 0, ai0: color.R, ai1: color.G, ai2: color.B);

                        float dir = (ctx.Target.Center - ctx.Npc.Center).ToRotation();
                        const float length = 16 * 90;
                        const float speed = length / (30 + 60 + 30);

                        for (int i = 0; i < 12; i++)
                        {
                            Vector2 direction = dir.ToRotationVector2();

                            ctx.Npc.NewProjectileDirectInAI_Server<ShiftStar>(ctx.Npc.Center, direction * speed, Helper.GetProjDamage(60, 80, 120), 0, ai0: 1);

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