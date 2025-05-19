using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        public bool GatherCurrent()
        {
            switch (SonState)
            {
                default:
                case 0://扇翅膀蓄力
                    {
                        if (Timer == 0)
                        {
                            NPC.frame.Y = 1;
                            Timer = 1;
                        }

                        NPC.velocity *= 0.8f;
                        SetSpriteDirectionFoTarget();
                        SetRotationNormally(0.2f);

                        //向后扇一下翅膀
                        if (++NPC.frameCounter > 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 7)
                            {
                                NPC.frame.Y = 0;
                                SonState = 1;
                                Timer = 0;
                                OpenMouse = true;
                                ResetAllOldCaches();
                                canDrawShadows = true;
                                currentSurrounding = true;

                                NPC.velocity = Vector2.Zero;

                                Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, NPC.Center, pitch: 0.4f);
                                SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);

                                Recorder = Main.rand.NextFloat(MathHelper.TwoPi);
                                Recorder2 = Main.rand.NextFloat(MathHelper.TwoPi);
                            }
                        }
                    }
                    break;
                case 1://不断生成电球
                    {
                        const int spawnTime = 24 * 10;
                        const int maxTime = 60 * 5 + 24 * 10;

                        float factor = Timer / maxTime;
                        shadowScale = Helper.Lerp(1f, 2.5f, (factor * 20) % 1);
                        shadowAlpha = Helper.Lerp(1f, 0f, (factor * 20) % 1);

                        Timer++;
                        if (Timer % 3 == 0)
                        {
                            float length2 = 800 - Timer / maxTime * 600;
                            Vector2 dir = Helper.NextVec2Dir();
                            Dust d = Dust.NewDustPerfect(NPC.Center + dir * (length2 + Main.rand.Next(1, 80))
                                , DustID.PortalBoltTrail, -dir * Main.rand.NextFloat(4, 8), newColor: ZacurrentDustRed, Scale: Main.rand.NextFloat(1.5f, 2f));
                            d.noGravity = true;
                        }

                        if (Timer%30==0)
                        {
                            float length2 = 800 - Timer / maxTime * 700;
                            for (int i = 0; i < 70; i++)
                            {
                                Vector2 dir = (i * MathHelper.TwoPi / 70).ToRotationVector2();
                                Dust d = Dust.NewDustPerfect(NPC.Center + dir * (length2 + Main.rand.Next(1, 80))
                                    , DustID.PortalBoltTrail, -dir * Main.rand.NextFloat(2, 5), newColor: ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }
                        }

                        if (Timer < spawnTime)
                        {
                            if (Timer % 10 == 0)
                            {
                                Recorder += MathHelper.TwoPi / 6 + 0.15f;
                                int length = Main.rand.Next(800, 900);

                                Vector2 pos = NPC.Center + Recorder.ToRotationVector2() * length;

                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, ModContent.NPCType<PurpleVoltBall>(), ai0: NPC.whoAmI);
                            }

                            if (Timer % (10 * 4) == 0)
                            {
                                Recorder2 += MathHelper.TwoPi / 3 + 0.3f;

                                int length = Main.rand.Next(550, 800);

                                Vector2 pos = NPC.Center + Recorder2.ToRotationVector2() * length;

                                NPC.NewProjectileInAI<RedVoltBall>(pos, Vector2.Zero, Helper.GetProjDamage(200, 250, 300), 0, NPC.target, NPC.whoAmI);
                                Helper.PlayPitched(CoraliteSoundID.QuietElectric_DD2_LightningAuraZap, NPC.Center);
                            }
                        }

                        if (Timer > maxTime)
                        {
                            SonState = 2;
                            Timer = 0;

                            canDrawShadows = false;
                            OpenMouse = false;
                        }
                    }
                    break;
                case 2://结束
                    {
                        FlyingFrame();
                        SetSpriteDirectionFoTarget();
                        TurnToNoRot();

                        Timer++;
                        if (Timer > 30)
                            return true;
                    }
                    break;
            }
            return false;
        }
    }
}
