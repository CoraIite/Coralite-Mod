using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Bosses.VanillaReinforce.EoC;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.MucuryBurst, typeof(ShadowBallContext))]
    public class MucuryBurstState : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.MucuryBurst;

        private enum Beat
        {
            Ready,                                        
            Move,
            Burst,
            Over,
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    {
                        ctx.SetVelocitySlowDown(0.95f);

                        if (Timer > 20)
                        {
                            if (ctx.Recorder2.GetProjectileOwner<MucuryBurst>(out Projectile burst))
                            {
                                Vector2 dir = (burst.rotation + MathHelper.Pi).ToRotationVector2();
                                ctx.Npc.velocity = dir * ctx.Recorder3_P2Only;
                            }
                            SwitchBeat(ctx, (int)Beat.Move);
                        }
                    }
                    break;
                case Beat.Move:
                    float f1 = Timer / 30f;
                    ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 5, 0.1f);
                    ctx.Npc.rotation += Helper.SinEase(f1) * 0.1f;
                    ctx.CoreOffset_AngledTo_P2(0.25f);
                    if (Timer > 30)
                    {
                        SwitchBeat(ctx, (int)Beat.Burst);
                    }

                    ctx.SetVelocitySlowDown(0.95f);
                    break;
                case Beat.Burst:
                    ctx.SetVelocitySlowDown(0.95f);

                    if (Timer<15)
                    {
                        ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 20, Timer / 15f);
                    }
                    else
                    {
                        ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 5, 0.1f);
                    }

                    ctx.CoreOffset_AngledTo_P2(0.25f);

                    //水银喷发
                    if (Timer > 80)
                    {
                        SwitchBeat(ctx, (int)Beat.Over);
                    }
                    break;
                case Beat.Over:
                    break;
                default:
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    {
                        if (Timer == 1)
                        {
                            float dir = (ctx.Target.Center - ctx.Npc.Center).ToRotation();
                            float distance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);

                            ctx.Recorder3_P2Only = 12 + Helper.Clamp(distance / (16 * 80), 0, 1) * 32;

                            Projectile p = ctx.Npc.NewProjectileDirectInAI_Server<MucuryBurst>(ctx.Npc.Center, (dir + MathHelper.Pi).ToRotationVector2(), Helper.GetProjDamage(80, 90, 100), 0, ai0: ctx.Npc.whoAmI, ai1: 30, ai2: 16 * ctx.Recorder3_P2Only);
                            ctx.Recorder2 = p.whoAmI;

                            Color color = Color.Silver * 0.4f;

                            ctx.Npc.NewProjectileDirectInAI_Server<AbsorbProj>(ctx.Npc.Center, Vector2.Zero, 0, 0, ai0: color.R, ai1: color.G, ai2: color.B);
                        }
                    }
                    break;
                case Beat.Move:
                    ctx.SetVelocitySlowDown(0.95f);
                    break;
                case Beat.Burst://水银喷发
                    {
                        if (Timer<5)
                        {
                            float baseRot = Timer * 0.25f + MathHelper.PiOver2;

                            for (int i = 1; i < 5; i++)
                            {
                                baseRot += MathHelper.TwoPi / 5f;

                                int style = Main.rand.Next(2);

                                float speed;
                                int flyTime;

                                if (style == 0)//大
                                {
                                    ctx.Recorder1++;
                                    speed = Main.rand.NextFloat(10, 16);
                                    flyTime = Main.rand.Next(15, 25);
                                }
                                else//小
                                {
                                    speed = Main.rand.NextFloat(4, 12);
                                    flyTime = Main.rand.Next(10, 20);
                                }

                                ctx.Npc.NewProjectileDirectInAI_Server<MucuryDroplet>(ctx.Npc.Center, (baseRot + Main.rand.NextFloat(-0.25f, 0.25f)).ToRotationVector2() * speed, Helper.GetProjDamage(80, 90, 100), 0, ai0: style, ai1: flyTime, ai2: ctx.Recorder1 * 20);
                            }
                        }
                    }
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
