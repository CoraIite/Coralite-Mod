using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        public bool AimThunderBall(int aimTime)
        {
            switch (SonState)
            {
                default:
                case 0://扇翅膀蓄力
                    {
                        if (Timer == 0)
                        {
                            NPC.frame.Y = 1;
                        }

                        NPC.velocity *= 0.8f;
                        SetSpriteDirectionFoTarget();
                        SetRotationNormally(0.2f);

                        float factor = Timer / 55;
                        float length = Helper.Lerp(80, 750, factor);

                        for (int i = 0; i < 2; i++)
                        {
                            PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2CircularEdge(length, length),
                                Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                        }

                        Timer++;
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

                                NPC.velocity = Vector2.Zero;

                                Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, NPC.Center, pitch: 0.4f);
                                SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);

                                Recorder = Main.rand.NextFloat(MathHelper.TwoPi);
                            }
                        }
                    }
                    return false;
                case 1://不断生成电球
                    {
                        Timer++;
                        if (Timer % 15 == 0)
                        {
                            int count = 3;

                            int damage = Helper.GetProjDamage(80, 140, 180);
                            for (int i = 0; i < count; i++)
                            {
                                Vector2 dir = (Recorder + Timer * 0.08f + i * MathHelper.TwoPi / count + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2();
                                NPC.NewProjectileInAI<AimThunderBall>(NPC.Center, dir * Main.rand.NextFloat(4, 8), damage, 0, NPC.target, NPC.whoAmI, aimTime + Timer / 2);
                            }

                            ElectricSound();
                        }

                        if (Timer > 15 * 3)
                            return true;
                    }
                    return false;
            }
        }
    }
}
