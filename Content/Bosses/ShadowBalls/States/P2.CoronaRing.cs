using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.CoronaRing, typeof(ShadowBallContext))]
    public class CoronaRing : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.CoronaRing;

        private enum Beat
        {
            Ready,
            Shoot,
            Wait,
            Over,
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    {
                        ctx.SetVelocitySlowDown(0.95f);
                        ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 0, Timer /20f);
                        ctx.CoreOffset_AngledTo_P2(0.25f);

                        if (Timer > 20)
                        {
                            SwitchBeat(ctx, (int)Beat.Shoot);
                        }
                    }
                    break;
                case Beat.Shoot:
                    ctx.SetVelocitySlowDown(0.95f);

                    if (Timer < 15)
                    {
                        ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 40, Timer / 15f);
                    }
                    else
                    {
                        ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 5, 0.01f);
                    }

                    ctx.CoreOffset_AngledTo_P2(0.25f);

                    //水银喷发
                    if (Timer > 15*ctx.Recorder1)
                    {
                        SwitchBeat(ctx, (int)Beat.Wait);
                    }
                    break;
                case Beat.Wait:
                    ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 5, 0.1f);
                    ctx.CoreOffset_AngledTo_P2(0.25f);

                    if (Timer > 100)
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
                case Beat.Ready:
                    {
                        if (Timer == 2)//设置初始值
                        {
                            ctx.Recorder1 = Helper.ScaleValueForDiffMode(3, 3, 4, 4) + Main.rand.Next(3);

                            ctx.Recorder2 = (ctx.Target.Center - ctx.Npc.Center).ToRotation();
                            //ctx.Recorder3_P2Only=
                        }
                    }
                    break;
                case Beat.Shoot://水银喷发
                    {
                        if (Timer % 15 == 0)
                        {
                            ctx.Npc.SimpleStrikeNPC((int)(ctx.Npc.lifeMax * 0.003f), 0);

                            int count = Timer / 15;
                            int round = (int)(count % 3);

                            float ringLength;
                            float ExAngle;

                            //第一个必定朝向玩家而且是短的所以基本上稍微有点距离都不会被打到，这是刻意的设计为了让前摇融入招式中QwQ
                            switch (round)
                            {
                                default:
                                case 0://短
                                    ringLength = Main.rand.Next( 8, 16);
                                    ExAngle = Main.rand.NextFloat(-1f, -0.5f);
                                    break;
                                case 1://长
                                    ringLength = Main.rand.Next(26, 48);
                                    ExAngle = Main.rand.NextFloat(-0.5f, 0.5f);
                                    break;
                                case 2://中
                                    ringLength = Main.rand.Next(20, 30);
                                    ExAngle = Main.rand.NextFloat(0.5f, 1f);
                                    break;
                            }

                            ringLength *= 16;
                            int damage = Helper.GetProjDamage(80, 90, 100);

                            //生成一个环弹幕
                            ctx.Npc.NewProjectileDirectInAI_Server<MucuryRingLaser>(ctx.Npc.Center, ctx.Npc.Center+(ctx.Recorder2 + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * ringLength, damage, 0, ai0: ctx.Npc.whoAmI, ai1: 0);

                            //生成激光，前3次必定不会射向玩家
                            float laserDir = (ctx.Recorder2 + Main.rand.NextFloat(-0.2f, 0.2f) + MathHelper.Pi + ExAngle);

                            if (count < 3 && MathF.Abs(Helper.AngleRad((ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero), laserDir.ToRotationVector2())) < 0.4f)
                            {
                                laserDir += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(0.6f, 0.9f);
                            }

                            ctx.Npc.NewProjectileDirectInAI_Server<MucuryLaserWithAim>(ctx.Npc.Center, laserDir.ToRotationVector2(), damage, 0, ai0: ctx.Npc.whoAmI, ai1: 40 + 20 * round, ai2: Vector2.Distance(ctx.Npc.Center, ctx.Target.Center));

                            ctx.Recorder2 += (MathHelper.TwoPi / 2) * Main.rand.NextFloat(0.6f, 1.6f);
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
