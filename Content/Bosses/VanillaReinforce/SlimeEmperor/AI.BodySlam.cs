using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void BodySlam()
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
                case 4: //瞬移到玩家头顶
                    {
                        if (Timer < 60) //生成粒子，准备瞬移
                        {
                            float factor = Timer / 60f;
                            float width = 80 - factor * 60;

                            for (int i = 0; i < 6; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(width, width), DustID.Teleporter,
                                    -NPC.velocity * Main.rand.NextFloat(0.2f, 0.4f), newColor: Coralite.Instance.MagicCrystalPink, Scale: Main.rand.NextFloat(1f, 1.5f));
                                dust.noGravity = true;
                            }

                            break;
                        }

                        if (Timer < 61) //生成粒子并传送到玩家头顶 
                        {
                            NPC.TargetClosest();
                            Vector2 oldCenter = NPC.Center;
                            NPC.Center = Target.Center + new Vector2(0, -450);
                            float length = (oldCenter - NPC.Center).Length();
                            Vector2 dir = (NPC.Center - oldCenter).SafeNormalize(Vector2.Zero);

                            SoundEngine.PlaySound(CoraliteSoundID.SlimeMount_Item81, NPC.Center);
                            for (int i = 0; i < length; i += 8)
                            {
                                int type = Main.rand.Next(3) switch
                                {
                                    0 => DustID.Teleporter,
                                    _ => DustID.TintableDust
                                };
                                Dust dust = Dust.NewDustPerfect(oldCenter + dir * i + Main.rand.NextVector2Circular(48, 48), type,
                                    dir * Main.rand.NextFloat(0.2f, 3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.5f, 2f));
                                dust.noGravity = true;
                            }

                            for (int i = 0; i < 24; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(80, 80), DustID.Teleporter,
                                   (i * (MathHelper.TwoPi / 24)).ToRotationVector2() * Main.rand.NextFloat(6, 8), newColor: Coralite.Instance.MagicCrystalPink, Scale: Main.rand.NextFloat(1f, 1.5f));
                                dust.noGravity = true;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int howMany = Helper.ScaleValueForDiffMode(1, 2, 2, 3);
                                for (int i = 0; i < howMany; i++)
                                {
                                    Point pos = oldCenter.ToPoint();
                                    pos.X += Main.rand.Next(-NPC.width, NPC.width);
                                    pos.Y += Main.rand.Next(-32, 32);
                                    NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos.X, pos.Y, ModContent.NPCType<ElasticGelBall>());
                                    npc.velocity = Helper.NextVec2Dir() * Main.rand.NextFloat(2, 5);
                                }
                            }

                            break;
                        }

                        SonState = 5;
                        Timer = 0;
                        NPC.TargetClosest();
                    }
                    break;
                case 5: //追踪一小会
                    {
                        if (Timer < 90)
                        {
                            Vector2 targetVec = Target.Center + new Vector2(0, -450) - NPC.Center;
                            Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);
                            float length = targetVec.Length();
                            if (length < 30)
                                NPC.velocity -= dir * 0.75f;
                            else
                                NPC.velocity += dir * 0.75f;

                            if (NPC.velocity.Length() > 10)
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 10;
                            break;
                        }

                        if (Timer < 120)
                        {
                            NPC.velocity *= 0.95f;
                            for (int i = 0; i < 4; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(80, 80), DustID.TintableDust,
                                    -NPC.velocity * Main.rand.NextFloat(0.2f, 0.4f), 150, new Color(78, 136, 255, 80), 2f);
                                dust.noGravity = true;
                            }

                            break;
                        }

                        SonState = 6;
                        Timer = 0;
                        SlimeMode();
                        Scale = new Vector2(0.8f, 1.25f);
                        NPC.noGravity = true;
                        NPC.noTileCollide = true;
                        NPC.velocity.X *= 0.5f;
                        NPC.velocity.Y = 8f;
                        NPC.frame.Y = 0;
                        NPC.TargetClosest();
                        SoundEngine.PlaySound(CoraliteSoundID.SlimeMount_Item81, NPC.Center);
                    }
                    break;
                case 6: //下砸
                    {
                        NPC.velocity.Y += 1.4f;
                        if (NPC.velocity.Y > 24)
                            NPC.velocity.Y = 24;
                        if (Timer == 12)
                            CanDrawShadow = true;

                        if (Timer < 120)   //检测下方物块
                        {
                            if (NPC.Center.Y > Target.Center.Y - 100)
                            {
                                Point position = NPC.BottomLeft.ToTileCoordinates();
                                int width = NPC.width / 16;
                                for (int i = 0; i < width; i++)
                                    for (int j = 1; j < 3; j++)
                                        if (WorldGen.ActiveAndWalkableTile(position.X + i, position.Y + j))    //砸地
                                        {
                                            NPC.rotation = 0;
                                            NPC.velocity *= 0;
                                            NPC.noGravity = false;
                                            NPC.noTileCollide = false;
                                            //生成砸地粒子
                                            SoundEngine.PlaySound(CoraliteSoundID.QueneSlimeFalling_Item167, NPC.Center);
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ProjectileID.QueenSlimeSmash, 40, 0f, Main.myPlayer);

                                            for (int l = 0; l < 20; l++)
                                            {
                                                int index = Dust.NewDust(NPC.Bottom - new Vector2(width / 2, 30f), width, 30, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 40, new Color(78, 136, 255, 80));
                                                Main.dust[index].noGravity = true;
                                                Main.dust[index].velocity.Y = -5f + Main.rand.NextFloat() * -3f;
                                                Main.dust[index].velocity.X *= 7f;
                                            }

                                            SonState = 7;
                                            return;
                                        }
                            }
                            break;
                        }

                        NPC.rotation = 0;
                        NPC.velocity *= 0;
                        NPC.noTileCollide = false;
                        NPC.noGravity = false;
                        SonState = 11;
                    }
                    break;
                default:
                case 7: //弹弹
                    ScaleToTarget(1.3f, 0.7f, 0.2f, Scale.Y < 0.75f, () => SonState = 8);
                    break;
                case 8:
                    ScaleToTarget(0.7f, 1.3f, 0.15f, Scale.X < 0.75f, () =>
                    {
                        CrownJumpUp(10, 3);
                        SonState = 9;
                    });
                    break;
                case 9:
                    ScaleToTarget(1.2f, 0.85f, 0.15f, Scale.Y < 0.9f, () => SonState = 10);
                    break;
                case 10:
                    ScaleToTarget(1f, 1f, 0.1f, Math.Abs(Scale.Y - 1) < 0.05f, () =>
                    {
                        Scale = Vector2.One;
                        SonState = 11;
                        Timer = 0;
                    });
                    break;
                case 11:
                    ResetStates();
                    break;
            }

            Timer++;
        }
    }
}
