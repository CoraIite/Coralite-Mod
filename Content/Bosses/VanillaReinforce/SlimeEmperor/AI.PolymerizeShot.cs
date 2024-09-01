using Coralite.Core;
using Coralite.Helpers;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public int PolymerizeTime = 240;

        public void PolymerizeShot()
        {
            switch ((int)SonState)
            {
                case 0://确保在地上
                    Jump(1f, 8f, onLanding: () => SonState = 1);
                    break;
                case 1:  //压缩
                    ScaleToTarget(1.2f, 0.9f, 0.2f, Scale.X > 1.15f, () =>
                    {
                        SonState++;
                        Timer = 0;
                        foreach (var npc in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<SlimeAvatar>()))
                        {
                            npc.ai[3] = -1;
                            npc.noGravity = true;
                            npc.velocity *= 0;
                        }
                    });
                    break;
                case 2:  //蓄力，之后射出一堆凝胶弹幕
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 4)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 3)
                                NPC.frame.Y = 0;
                        }

                        if (Timer < PolymerizeTime)
                        {
                            float factor = Timer / (float)PolymerizeTime;

                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 dir = Helper.NextVec2Dir();
                                Vector2 vel = dir.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.3f, 2f);
                                Dust dust = Dust.NewDustPerfect(NPC.Center + (dir * (200 - (factor * 180))) + Main.rand.NextVector2Circular(30, 30), DustID.t_Slime
                                    , vel, 150, new Color(78, 136, 255, 80), 2f);
                                dust.noGravity = true;
                            }

                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 dir = Helper.NextVec2Dir();
                                Vector2 vel = -dir * Main.rand.NextFloat(4f, 8f);
                                Dust dust = Dust.NewDustPerfect(NPC.Center + (dir * (300 - (factor * 240))), DustID.LastPrism
                                    , vel, 150, new Color(78, 136, 255, 80), 1.2f);
                                dust.noGravity = true;
                            }

                            break;
                        }

                        int damage = Helper.GetProjDamage(20, 25, 30);
                        float scale = 1f;
                        float howMany = 3f;
                        float speed = 10f;
                        int type = ModContent.NPCType<SlimeAvatar>();
                        foreach (var npc in Main.npc.Where(n => n.active && n.type == type))
                        {
                            damage += 10;
                            scale += 0.2f;
                            howMany += 1f;
                            speed += 1f;
                            Vector2 dir = (NPC.Center - npc.Center).SafeNormalize(Vector2.Zero);
                            for (int i = 0; i < 8; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * Main.rand.Next(70)) + Main.rand.NextVector2Circular(48, 48),
                                      DustID.t_Slime, dir * Main.rand.NextFloat(2f, 6.5f), 150, new Color(78, 136, 255, 80), 1.2f);
                                dust.noGravity = true;
                            }
                            npc.Kill();
                        }

                        if (speed > 28)
                            speed = 28;
                        if (scale > 4f)
                            scale = 4f;
                        if (howMany > 25)
                            howMany = 25;
                        Vector2 direction = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

                        SoundEngine.PlaySound(CoraliteSoundID.QueneSlimeFalling_Item167, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < howMany; i++)
                            {
                                Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(speed - 2, speed + 2),
                                     ModContent.ProjectileType<GelProj>(), damage, 4f, NPC.target);
                                p.scale = scale;
                                p.width = (int)(p.width * scale);
                                p.height = (int)(p.height * scale);
                                p.netUpdate = true;
                            }

                            for (int i = -1; i < 2; i++)
                            {
                                Point pos = NPC.Center.ToPoint();
                                pos.X += Main.rand.Next(-32, 32);
                                pos.Y += Main.rand.Next(-32, 32);
                                NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos.X, pos.Y, ModContent.NPCType<ElasticGelBall>());
                                npc.velocity = direction.RotatedBy(i * 0.3f) * Main.rand.NextFloat(4, 8);
                            }
                        }

                        howMany *= 8;
                        for (int i = 0; i < howMany; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(64, 64),
                                  DustID.t_Slime, direction * Main.rand.NextFloat(2f, 8f), 150, new Color(78, 136, 255, 80), 2f);
                            dust.noGravity = true;
                        }

                        SonState = 3;
                        Timer = 0;
                    }
                    break;
                case 3: //后摇
                    ScaleToTarget(0.75f, 1.35f, 0.2f, Scale.Y > 1.3f, () =>
                    {
                        SonState++;
                        Timer = 0;
                    });
                    break;
                case 4:
                    ScaleToTarget(1.2f, 0.9f, 0.15f, Scale.X > 1.15f, () =>
                    {
                        SonState++;
                        Timer = 0;
                    });
                    break;
                case 5:
                    ScaleToTarget(1f, 1f, 0.1f, Math.Abs(Scale.X - 1) < 0.05f, () =>
                    {
                        SonState++;
                        Timer = 0;
                    });
                    break;
                default:
                case 6:
                    ResetStates();
                    break;
            }

            Timer++;
        }
    }
}
