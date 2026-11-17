using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.UnstableEruption, typeof(ShadowBallContext))]
    public class UnstableEruptionState : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.UnstableEruption;

        private enum Beat
        {
            Ready,
            MultiroundEruption,
            SlowDown,
            Over,
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备阶段，随机初始值
                    ctx.Recorder1 = Helper.ScaleValueForDiffMode(0, 0, 1, 2) + ctx.Rand().Next(2, 4);
                    ctx.Recorder2 = -1;
                    ctx.SetRotationLetItGo();

                    SwitchBeat(ctx,(int)Beat.MultiroundEruption);
                    break;
                case Beat.MultiroundEruption:
                    ctx.Npc.noTileCollide = false;
                    if (ctx.Recorder2.GetProjectileOwner<MucuryBurst>(out Projectile burst))
                    {
                        //朝向目标加速
                        Vector2 dir = (burst.rotation + MathHelper.Pi).ToRotationVector2();

                        if (Timer<=30)
                        {
                            ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 0, 0.1f);
                            ctx.Npc.rotation +=Timer / 30f * 0.15f;
                            ctx.CoreOffset_AngledTo_P2(0.5f*Timer/30f);

                            ctx.SetVelocitySlowDown(0.95f);
                        }
                        else if (Timer <= 30 + 60)
                        {
                            ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 10, 0.2f);
                            ctx.Npc.rotation += 0.15f;
                            ctx.CoreOffset_AngledTo_P2(0.5f);

                            ctx.SetVelocityLetItGo();

                            float f = (Timer - 30f) / 60f;
                            ctx.Npc.velocity = dir * Helper.X2Ease(f) * 24;

                            if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > 16 * 60)
                                Timer = 30 + 61;
                        }
                        else
                        {
                            ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.05f);

                            ctx.CoreOffset_AngledTo_P2(0.1f);

                            ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 5, 0.1f);
                            ctx.SetVelocitySlowDown(0.95f);
                        }
                    }


                    if (Timer > 90 + 30)
                    {
                        ctx.Recorder1--;
                        SwitchBeat(ctx, (int)Beat.SlowDown);
                    }
                    break;
                case Beat.SlowDown:
                    ctx.Npc.rotation += Helper.SinEase(Timer / 15f) * 0.1f;
                    ctx.CoreOffset_AngledTo_P2(0.3f);

                    if (Timer > 15)
                    {
                        SwitchBeat(ctx, (int)(ctx.Recorder1 < 1 ? Beat.Over : Beat.MultiroundEruption));
                    }
                    break;
                case Beat.Over:

                    break;
                default:
                    SwitchBeat(ctx, (int)Beat.Ready);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.MultiroundEruption:
                    if (Timer == 1)
                    {
                        float dir = (ctx.Target.Center - ctx.Npc.Center).ToRotation()+Main.rand.NextFromList(-0.4f,0,0.4f);

                       Projectile p= ctx.Npc.NewProjectileDirectInAI_Server<MucuryBurst>(ctx.Npc.Center, (dir + MathHelper.Pi).ToRotationVector2(), Helper.GetProjDamage(80, 90, 100), 0, ai0: ctx.Npc.whoAmI, ai1: 90, ai2: 16 * 25);
                        ctx.Recorder2 = p.whoAmI;
                    }

                    if (ctx.Recorder2.GetProjectileOwner<MucuryBurst>(out Projectile burst))
                    {
                        if (Timer <= 30)
                        {
                        }
                        else if (Timer <= 30 + 90)
                        {
                            if (Timer % 15 == 0)
                            {
                                for (int i = -1; i < 2; i += 2)
                                {
                                    Vector2 pos = ctx.Npc.Center;

                                    ctx.Npc.NewProjectileDirectInAI_Server<ShadowStar>(pos, Vector2.Zero, Helper.GetProjDamage(60, 65, 100), 0, ai0: 2, ai1: burst.rotation, ai2: i * 0.01f);
                                }
                            }
                        }
                    }
                    break;
                case Beat.SlowDown:
                    break;
                case Beat.Over:
                    return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P2Test);
                default:
                    break;
            }

            return null;
        }
    }
}
