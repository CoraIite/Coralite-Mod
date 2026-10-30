using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStateId.Gravity, typeof(ShadowBallContext))]
    public sealed class ShadowBallGravityState : ShadowBallStateBase
    {
        private enum Beat
        {
            Flash,
            Gather,
            ReadyToShoot,
            Shoot,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.Gravity;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            const float damping = 0.95f;
            const float pullDistance = 16 * 60;

            if ((Beat)BeatIndex is Beat.Flash or Beat.Gather)
                ctx.SetVelocitySlowDown(damping);
            else
                ctx.SetVelocityLetItGo();

            if ((Beat)BeatIndex != Beat.Gather)
                return;

            if (Timer < 10 || Timer % 60 > 10)
            {
                return;
            }

            if (!VaultUtils.isServer&& Vector2.DistanceSquared(Main.LocalPlayer.Center, ctx.Npc.Center) <= pullDistance * pullDistance)
            {
                Main.LocalPlayer.velocity += (ctx.Npc.Center - Main.LocalPlayer.Center).SafeNormalize(Vector2.Zero) * 0.5f;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            const int stoneSpawnTime = 12;
            const int callInterval = 35;
            const int StoneSpawnTimer = 12 * 16;
            const int gatherFrames = 28 * 16;
            const int shootFrames = 80;

            switch ((Beat)BeatIndex)
            {
                case Beat.Flash:
                    if (Timer == 2)
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);

                        ctx.Npc.velocity = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero) * Helper.Clamp((ctx.Target.Center - ctx.Npc.Center).Length() / 16, 0, 30);
                        ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Gravity, ctx.Boss.smallBalls.Count);
                    }

                    ctx.Npc.velocity *= 0.97f;
                    ctx.Boss.LockDistancePercent = 1 + 1 * Helper.SqrtEase(Timer / 30f);

                    if (Timer >= 30)
                    {
                        ctx.Boss.GetSmallBalls();
                        SwitchBeat(ctx, (int)Beat.Gather);
                    }
                    break;
                case Beat.Gather://吸引石头并控制小球射激光
                    ctx.Npc.velocity *= 0.95f;
                    ctx.Boss.LockDistancePercent = 2 - (float)Timer / gatherFrames;
                    ctx.Boss.CommonShellBreath(2);

                    if (Timer % 60 == 0)
                    {
                        Color color = Color.Gray * 0.5f;
                        ctx.Npc.NewProjectileDirectInAI_Server<AbsorbProj>(ctx.Npc.Center, Vector2.Zero, 0, 0,
                            ai0: color.R, ai1: color.G, ai2: color.B);
                    }

                    if (Timer % stoneSpawnTime == 0 && Timer < StoneSpawnTimer)
                    {
                        Player target = ctx.Target;

                        Vector2 away = Timer % (stoneSpawnTime * 2) == 0 ? (target.Center - ctx.Npc.Center).SafeNormalize(Vector2.UnitY).RotateByRandom(-0.5f, 0.5f) : Helper.NextVec2Dir();
                        ctx.Npc.NewProjectileDirectInAI_Server<GravityStone>(target.Center + away * Main.rand.NextFloat(1000, 1400),
                            -away * 0.5f, Helper.GetProjDamage(65, 80, 100), 0,
                            ai0: target.ZoneSnow ? 1 : 0, ai1: ctx.Npc.whoAmI);
                    }

                    if (Timer % callInterval == 0&&Timer< gatherFrames-50)
                    {
                        ctx.Boss.GetSmallBalls();
                        List<SmallShadowBall> waiting = [];
                        foreach (NPC ball in ctx.Boss.smallBalls)
                            if (ball.ModNPC is SmallShadowBall smallBall
                                && smallBall.CurrentStateId == (int)SmallShadowBallStateId.Gravity
                                && smallBall.GravityBeat == 0)
                                waiting.Add(smallBall);

                        if (waiting.Count > 0)
                            Main.rand.Next(waiting).ServerSetGravityBeat(1);
                    }

                    if (Timer >= gatherFrames)
                    {
                        ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Idle, ctx.Boss.smallBalls.Count);
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCirclesAngled);

                        SwitchBeat(ctx, (int)Beat.ReadyToShoot);
                    }
                    break;
                case Beat.ReadyToShoot:
                    ctx.Boss.LockDistancePercent = 1 - 0.4f * Timer / 80;
                    ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(),0.15f);


                    if (Timer >= 80)
                    {
                        int stoneCount = 0;
                        foreach (Projectile projectile in Main.ActiveProjectiles)
                        {
                            if (projectile.type != ModContent.ProjectileType<GravityStone>()
                                || projectile.ai[1] != ctx.Npc.whoAmI)
                                continue;

                            ((GravityStone)projectile.ModProjectile).TurnToShoot(ctx.Npc.target, stoneCount * 5);
                            stoneCount++;
                        }

                        SwitchBeat(ctx, (int)Beat.Shoot);
                    }
                    break;
                case Beat.Shoot:
                    ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.15f);

                    if (Timer < shootFrames / 7)
                    {
                        ctx.Boss.LockDistancePercent = 0.5f + 1.5f * Timer / (shootFrames / 7f);
                    }
                    else
                    {
                        ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 1, 0.05f);
                    }
                    if (Timer >= shootFrames)
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);

                        ctx.Boss.LockDistancePercent = 1f;
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.P1Test);
                    }
                    break;
            }

            return null;
        }
    }
}
