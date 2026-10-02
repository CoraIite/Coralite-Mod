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
            Shoot,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.Gravity;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            const float damping = 0.95f;
            const float pullDistance = 16 * 20;

            if ((Beat)BeatIndex is Beat.Flash or Beat.Gather)
                ctx.DeclareDamp(damping);
            else
                ctx.DeclareKeep();

            if ((Beat)BeatIndex != Beat.Gather)
                return;

            if (Timer%45!=0)
            {
                return;
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead || Vector2.DistanceSquared(player.Center, ctx.Npc.Center) <= pullDistance * pullDistance)
                    continue;

                // Only the player's local simulation applies the pull in multiplayer.
                if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient && player.whoAmI != Main.myPlayer)
                    continue;

                player.velocity += (ctx.Npc.Center - player.Center).SafeNormalize(Vector2.Zero) * 6f;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            const int flashFrames = 30;
            const int stoneInterval = 10;
            const int callInterval = 40;
            const int StoneSpawnTimer = 12 * 16;
            const int gatherFrames = 28 * 16;
            const int shootFrames = 80;

            switch ((Beat)BeatIndex)
            {
                case Beat.Flash:
                    // TODO: Spawn the white flash effect.
                    if (Timer >= flashFrames)
                    {
                        ctx.Boss.GetSmallBalls();
                        ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Gravity, ctx.Boss.smallBalls.Count);
                        SwitchBeat(ctx, (int)Beat.Gather);
                    }
                    break;

                case Beat.Gather:
                    if (Timer % stoneInterval == 0 && Timer < StoneSpawnTimer)
                    {
                        Player target = ctx.Target;
                        if (target.active && !target.dead)
                        {
                            Vector2 away = (target.Center - ctx.Npc.Center).SafeNormalize(Vector2.UnitY).RotateByRandom(-0.5f, 0.5f);
                            ctx.Npc.NewProjectileDirectInAI_Server<GravityStone>(target.Center + away * Main.rand.NextFloat(1000, 1400),
                                -away * 0.5f, Helper.GetProjDamage(65,80,100), 0,
                                ai0: target.ZoneSnow ? 1 : 0, ai1: ctx.Npc.whoAmI);
                        }
                    }

                    if (Timer % callInterval == 0)
                    {
                        ctx.Boss.GetSmallBalls();
                        List<SmallShadowBall> waiting = [];
                        foreach (NPC ball in ctx.Boss.smallBalls)
                            if (ball.active && ball.ModNPC is SmallShadowBall smallBall
                                && smallBall.CurrentStateId == (int)SmallShadowBallStateId.Gravity
                                && smallBall.GravityBeat == 0)
                                waiting.Add(smallBall);

                        if (waiting.Count > 0)
                            Main.rand.Next(waiting).ServerSetGravityBeat(1);
                    }

                    if (Timer >= gatherFrames)
                    {
                        int stoneCount = 0;
                        foreach (Projectile projectile in Main.ActiveProjectiles)
                        {
                            if (projectile.type != ModContent.ProjectileType<GravityStone>()
                                || projectile.ai[1] != ctx.Npc.whoAmI)
                                continue;

                            ((GravityStone)projectile.ModProjectile).TurnToShoot(ctx.Npc.target, stoneCount*15);
                            stoneCount++;
                        }

                        SwitchBeat(ctx, (int)Beat.Shoot);
                    }
                    break;

                case Beat.Shoot:
                    if (Timer >= shootFrames)
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.OnSpawnAnim);
                    break;
            }

            return null;
        }
    }
}
