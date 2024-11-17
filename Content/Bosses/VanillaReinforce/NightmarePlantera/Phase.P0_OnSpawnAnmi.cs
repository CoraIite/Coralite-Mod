using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public partial class NightmarePlantera
    {
        public void OnSpawnAnmi()
        {
            UpdateFrameNormally();
            switch (State)
            {
                default:
                    ResetStates();
                    break;
                case 0: //吸收一些裂缝
                    {
                        if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                        {
                            if (NCamera.factor < 1)
                            {
                                NCamera.useScreenMove = true;
                                NCamera.factor += 0.03f;
                                if (NCamera.factor > 1)
                                    NCamera.factor = 1;
                            }
                        }

                        for (int i = 0; i < 2; i++)
                        {
                            Color color = Main.rand.Next(0, 2) switch
                            {
                                0 => new Color(110, 68, 200),
                                _ => new Color(122, 110, 134)
                            };

                            PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir(1, 3f),
                                CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1f));
                        }

                        //if (Timer == 2)
                        //{
                        //    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        //    for (int i = 0; i < 7; i++)
                        //    {
                        //        NPC.NewProjectileInAI<NightmareEnergy>(NPC.Center + angle.ToRotationVector2() * 750, (angle + MathHelper.PiOver2).ToRotationVector2() * 5, 1, 1, -1, i);
                        //        angle += MathHelper.TwoPi / 7;
                        //    }
                        //}

                        if (Timer > 2 && Timer % 8 == 0)
                        {
                            float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                            int howMany = Main.rand.Next(6);
                            for (int i = 0; i < howMany; i++)
                            {
                                float distance = Main.rand.Next(400, 600);
                                NPC.NewProjectileInAI<NightmareSpawnEnergy>(NPC.Center + ((angle + (i * MathHelper.TwoPi / howMany)).ToRotationVector2() * distance)
                                    , Vector2.Zero, 1, 1, -1, -1, distance, 1);
                            }
                        }

                        if (Timer > 60)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, NPC.Center);
                            canDrawWarp = true;
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 1:
                    {
                        NPC.rotation += 0.03f;

                        if (Main.rand.NextBool())
                        {
                            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(300, 300), ModContent.DustType<NightmareStar>(),
                                new Vector2(0, Main.rand.NextFromList(-1, 1) * 4), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                        }

                        if (Timer < 60)
                        {
                            if (Timer % 15 == 0)
                            {
                                //float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                                //NPC.NewProjectileInAI<NightmareEnergy>(NPC.Center + angle.ToRotationVector2() * Main.rand.Next(400, 600), (angle + 2.5f).ToRotationVector2() * 5, 1, 1, -1, -1);
                                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                                int howMany = Main.rand.Next(6);
                                for (int i = 0; i < howMany; i++)
                                {
                                    float distance = Main.rand.Next(200, 300);
                                    NPC.NewProjectileInAI<NightmareSpawnEnergy>(NPC.Center + ((angle + (i * MathHelper.TwoPi / howMany)).ToRotationVector2() * distance)
                                        , Vector2.Zero, 1, 1, -1, -1, distance, 1);
                                }
                            }

                            for (int i = 0; i < 2; i++)
                            {
                                Color color = Main.rand.Next(0, 2) switch
                                {
                                    0 => new Color(110, 68, 200),
                                    _ => new Color(122, 110, 134)
                                };

                                PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir(1, 3f),
                                    CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1f));
                            }

                            warpScale += 1 / 60f;
                            break;
                        }

                        if (Timer < 100)
                        {
                            alpha += 0.75f * 1 / 60f;
                            NPC.rotation += 0.02f;
                            break;
                        }

                        Vector2 dir = Helper.NextVec2Dir();
                        Dust dust = Dust.NewDustPerfect(NPC.Center + (dir * Main.rand.NextFloat(128f)), ModContent.DustType<NightmareDust>(), dir * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;

                        if (Timer % 2 == 0)
                        {
                            dir = Helper.NextVec2Dir();
                            Dust.NewDustPerfect(NPC.Center + (dir * Main.rand.NextFloat(128f)), DustID.VilePowder, dir * Main.rand.NextFloat(8f, 16f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 1.3f));
                        }

                        if (Timer < 140)
                            break;

                        if (Timer < 200)
                        {
                            warpScale -= 1 / 60f;
                        }

                        if (Timer > 200)
                        {
                            State++;
                            Timer = 0;
                            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, NPC.Center, pitch: -0.5f);

                            float angle = Main.rand.NextFloat(6.282f);
                            for (int i = 0; i < 3; i++)
                            {
                                NPC.NewProjectileDirectInAI<NightmareBurst>(NPC.Center, angle.ToRotationVector2() * 48, 1, 1, -1, angle);
                                angle += MathHelper.TwoPi / 3;
                            }

                            for (int i = 0; i < 12; i++)
                            {
                                float tor = Main.rand.NextFloat(6.282f);
                                NPC.NewProjectileDirectInAI<NightmareBurst>(NPC.Center, tor.ToRotationVector2() * Main.rand.Next(10, 20), 1, 1, -1, tor);
                            }

                            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                            {
                                NCamera.useShake = true;
                                NCamera.shakeLevel = 5;
                                NCamera.ShakeVec2 = Helper.NextVec2Dir();
                            }

                            Music = MusicID.Plantera;
                        }
                    }
                    break;
                case 2:
                    {
                        if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                        {
                            if (NCamera.factor > 0)
                            {
                                NCamera.useScreenMove = true;
                                NCamera.factor -= 1 / 20f;
                                if (NCamera.factor < 0)
                                    NCamera.factor = 0;
                            }
                        }

                        alpha += 0.75f * 1 / 20f;
                        warpScale += 0.3f;
                        NPC.rotation += 0.04f;

                        for (int i = 0; i < 6; i++)
                        {
                            Color color = Main.rand.Next(0, 2) switch
                            {
                                0 => new Color(110, 68, 200),
                                _ => new Color(122, 110, 134)
                            };

                            PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir(8, 26f),
                                CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 2f));
                        }

                        if (Timer > 20)
                        {
                            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera1))
                                NCamera1.Reset();

                            SetPhase1Idle();
                        }
                    }
                    break;
            }

            Timer++;
        }
    }

    public class SpawnLight : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
        }

        public override void AI()
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
