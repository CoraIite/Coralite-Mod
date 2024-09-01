using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void TransportSplit()
        {
            switch ((int)SonState)
            {
                case 0: //缩小的阶段
                    {
                        if (Timer < 2)
                            NPC.noGravity = true;

                        OnChangeToCrownMode();
                        ScaleToTarget(1.1f, 0.7f, 0.1f, Scale.Y < 0.75f || Timer > 20, () =>
                        {
                            SonState = 1;
                            Timer = 0;
                        });
                    }
                    break;
                case 1:
                    OnChangeToCrownMode();
                    ScaleToTarget(0.6f, 0.8f, 0.1f, Scale.X < 0.65f || Timer > 20, () =>
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                        SonState = 2;
                        Timer = 0;
                    });
                    break;
                case 2:
                    OnChangeToCrownMode();
                    ScaleToTarget(0.7f, 0.5f, 0.1f, Scale.Y < 0.55f || Timer > 20, () =>
                    {
                        SonState = 3;
                        Timer = 0;
                    });
                    break;
                case 3:
                    OnChangeToCrownMode();
                    ScaleToTarget(0.4f, 0.6f, 0.1f, Scale.X < 0.45f || Timer > 20, () =>
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                        SonState = 4;
                        Timer = 0;
                        CrownMode();
                        NPC.TargetClosest();
                    });
                    break;
                case 4: //瞬移到玩家附近
                    {
                        if (Timer < 60) //生成粒子，准备瞬移
                        {
                            float factor = Timer / 60f;
                            float width = 80 - (factor * 60);

                            for (int i = 0; i < 6; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(width, width), DustID.Teleporter,
                                    -NPC.velocity * Main.rand.NextFloat(0.2f, 0.4f), newColor: Coralite.MagicCrystalPink, Scale: Main.rand.NextFloat(1f, 1.5f));
                                dust.noGravity = true;
                            }

                            break;
                        }

                        if (Timer < 61) //生成粒子并传送到玩家附近
                        {
                            NPC.TargetClosest();
                            Vector2 oldCenter = NPC.Center;
                            NPC.Center = new Vector2(Target.Center.X + ((oldCenter.X - Target.Center.X) / 2), Target.Center.Y - 200);
                            float length = (oldCenter - NPC.Center).Length();
                            Vector2 dir = (NPC.Center - oldCenter).SafeNormalize(Vector2.Zero);

                            SoundEngine.PlaySound(CoraliteSoundID.SlimeMount_Item81, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int howMany = Helper.ScaleValueForDiffMode(1, 1, 2, 2);
                                for (int i = 0; i < howMany; i++)
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)oldCenter.X, (int)oldCenter.Y, ModContent.NPCType<SlimeAvatar>(), Target: NPC.target, ai1: NPC.whoAmI);
                            }
                            for (int i = 0; i < length; i += 8)
                            {
                                int type = Main.rand.Next(3) switch
                                {
                                    0 => DustID.Teleporter,
                                    _ => DustID.TintableDust
                                };
                                Dust dust = Dust.NewDustPerfect(oldCenter + (dir * i) + Main.rand.NextVector2Circular(48, 48), type,
                                    dir * Main.rand.NextFloat(0.2f, 3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.5f, 2f));
                                dust.noGravity = true;
                            }

                            for (int i = 0; i < 24; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(80, 80), DustID.Teleporter,
                                   (i * (MathHelper.TwoPi / 24)).ToRotationVector2() * Main.rand.NextFloat(6, 8), newColor: Coralite.MagicCrystalPink, Scale: Main.rand.NextFloat(1f, 1.5f));
                                dust.noGravity = true;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int howMany = Helper.ScaleValueForDiffMode(1, 2, 4, 6);
                                for (int i = 0; i < howMany; i++)
                                {
                                    Point pos = NPC.Center.ToPoint();
                                    pos.X += Main.rand.Next(-NPC.width, NPC.width);
                                    pos.Y += Main.rand.Next(-32, 32);
                                    NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos.X, pos.Y, ModContent.NPCType<ElasticGelBall>());
                                    npc.velocity = Helper.NextVec2Dir(2, 5);
                                }
                            }

                            break;
                        }

                        SonState = 5;
                        Timer = 0;
                        NPC.TargetClosest();
                    }
                    break;
                case 5:
                    {
                        //和玩家保持一定距离
                        if (Timer < 40)
                        {
                            Vector2 targetVec = Target.Center - NPC.Center;
                            Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);
                            float length = targetVec.Length();
                            if (length < 400)
                                NPC.velocity -= dir * 0.5f;
                            else
                                NPC.velocity += dir * 0.5f;

                            if (NPC.velocity.Length() > 8)
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 8;

                            break;
                        }

                        NPC.velocity.X *= 0;
                        SonState = 6;
                        Timer = 0;
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                        SlimeMode();
                    }
                    break;
                case 6:
                    OnChangeToCrownMode();
                    ScaleToTarget(1f, 1f, 0.15f, Scale.X > 0.97f, () =>
                    {
                        Scale = Vector2.One;
                        SonState = 7;
                        Timer = 0;
                    });
                    break;
                default:
                case 7:
                    ResetStates();
                    break;
            }

            Timer++;
        }
    }
}
