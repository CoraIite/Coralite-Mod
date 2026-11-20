using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
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

                        if (Timer > 30)
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

                    if (Timer>30)
                    {
                        SwitchBeat(ctx, (int)Beat.Burst);
                    }

                    ctx.SetVelocitySlowDown(0.95f);
                    break;
                case Beat.Burst:
                    ctx.SetVelocitySlowDown(0.95f);

                    //水银喷发
                    if (Timer>80)
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

                            ctx.Recorder3_P2Only = 12 + Helper.Clamp(distance / (16 * 80), 0, 1) * 22;

                            Projectile p = ctx.Npc.NewProjectileDirectInAI_Server<MucuryBurst>(ctx.Npc.Center, (dir + MathHelper.Pi).ToRotationVector2(), Helper.GetProjDamage(80, 90, 100), 0, ai0: ctx.Npc.whoAmI, ai1: 30, ai2: 16 * ctx.Recorder3_P2Only);
                            ctx.Recorder2 = p.whoAmI;
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
                                    speed = Main.rand.NextFloat(8, 12);
                                    flyTime = Main.rand.Next(25, 35);
                                }
                                else//小
                                {
                                    speed = Main.rand.NextFloat(10, 4);
                                    flyTime = Main.rand.Next(20, 30);
                                }

                                ctx.Npc.NewProjectileDirectInAI_Server<MucuryDroplet>(ctx.Npc.Center, (baseRot + Main.rand.NextFloat(-0.25f, 0.25f)).ToRotationVector2() * speed, Helper.GetProjDamage(80, 90, 100), 0, ai0: style, ai1: flyTime,ai2: ctx.Recorder1*25);
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
