using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.Gravity, typeof(ShadowBallContext))]
    public sealed class ShadowBallGravityState : ShadowBallStateBase
    {
        private enum Beat
        {
            Flash,
            Gather,
            ReadyToShoot,
            Shoot,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.Gravity;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ShadowBall boss = ctx.Boss;
            NPC npc = ctx.Npc;

            switch ((Beat)BeatIndex)
            {
                case Beat.Flash:
                    if (Timer == 0)
                    {
                        boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);

                        npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * Helper.Clamp((ctx.Target.Center - npc.Center).Length() / 25, 0, 30);
                        boss.CommandSmallBalls(SmallShadowBallStateId.Gravity, boss.smallBalls.Count);
                        ctx.SetVelocityLetItGo();
                        break;
                    }

                    boss.LockDistancePercent = 1 + 1 * Helper.SqrtEase(Timer / 30f);

                    ctx.SetVelocitySlowDown(0.95f);
                    ctx.SetRotationAdd(0.1f * Helper.SinEase(Timer / 30f));//逐渐自转
                    boss.ShellBreath_HideCore(2);
                    ctx.CoreOffsetAngle = npc.velocity.ToRotation();
                    ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, npc.velocity.Length(), 0.2f);

                    if (Timer >= 30)
                    {
                        boss.GetSmallBalls();
                        SwitchBeat(ctx, (int)Beat.Gather);
                    }

                    break;
                case Beat.Gather:
                    {
                        const int gatherFrames = 28 * 16;

                        ctx.SetVelocitySlowDown(0.95f);
                        boss.LockDistancePercent = 2 - (float)Timer / gatherFrames;
                        boss.CommonShellBreath(1);

                        if (Timer % 20 == 0)//依旧随机旋转
                        {
                            ctx.CoreOffsetAngle += MathHelper.Pi + Main.rand.NextFloat(-0.9f, 0.9f);
                            ctx.CoreOffsetRadius = Main.rand.Next(10, 40);

                            float f = (Timer % 20) / 20f;
                            ctx.SetRotationAdd(0.3f * f);//随机动的时候增加旋转
                        }
                        ctx.CoreOffset_AngledTo(0.15f);

                        if (Timer > 10 && Timer % 60 < 10)
                        {
                            const float pullDistance = 16 * 60;

                            if (!VaultUtils.isServer && Vector2.DistanceSquared(Main.LocalPlayer.Center, ctx.Npc.Center) <= pullDistance * pullDistance)
                            {
                                Main.LocalPlayer.velocity += (ctx.Npc.Center - Main.LocalPlayer.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                            }
                        }

                        if (Timer >= gatherFrames)
                        {
                            boss.CommandSmallBalls(SmallShadowBallStateId.Idle, boss.smallBalls.Count);
                            boss.SwitchLockState(ShadowBall.LockStates.ConcentricCirclesAngled);

                            SwitchBeat(ctx, (int)Beat.ReadyToShoot);
                        }
                    }
                    break;
                case Beat.ReadyToShoot:
                    boss.LockDistancePercent = 1 - 0.4f * Timer / 80;
                    ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.15f);
                    ctx.CoreOffset_BackToCenter(0.2f);
                    boss.ShellBreath_HideCore(2);

                    if (Timer > 80)
                    {
                        SwitchBeat(ctx, (int)Beat.Shoot);
                    }

                    break;
                case Beat.Shoot:
                    const int shootFrames = 80;

                    ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.15f);


                    if (Timer < shootFrames / 7)
                    {
                        boss.ShellBreath_HideCore(0);

                        boss.LockDistancePercent = 0.5f + 1.5f * Timer / (shootFrames / 7f);
                    }
                    else
                    {
                        boss.ShellBreath_ShowCore(0);
                        boss.LockDistancePercent = Helper.Lerp(boss.LockDistancePercent, 1, 0.05f);
                    }

                    if (Timer == shootFrames)
                    {
                        boss.SwitchLockState(ShadowBall.LockStates.Normal);

                        boss.LockDistancePercent = 1f;
                    }

                    break;
                default:
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            const int shootFrames = 80;

            switch ((Beat)BeatIndex)
            {
                case Beat.Gather://吸引石头并控制小球射激光
                    const int gatherFrames = 28 * 16;
                    const int stoneSpawnTime = 12;
                    const int StoneSpawnTimer = 12 * 16;

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

                    int sperateTime = Helper.ScaleValueForDiffMode(40, 35, 35, 30);

                    if (Timer % sperateTime == 0 && Timer < gatherFrames - 50)
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

                    break;
                case Beat.ReadyToShoot:
                    if (Timer == 80)
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
                    }
                    break;
                case Beat.Shoot:

                    if (Timer > shootFrames)
                    {
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                    }
                    break;
            }

            return null;
        }
    }
}
