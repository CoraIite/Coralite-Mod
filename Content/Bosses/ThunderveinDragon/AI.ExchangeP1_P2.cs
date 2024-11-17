using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void ExchangeP1_P2()
        {
            const int BurstTime = 40;
            const int RoaringTime = 60;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://收起翅膀准备
                    {
                        NPC.velocity *= 0.8f;
                        NPC.QuickSetDirection();
                        TurnToNoRot();
                        if (Timer < 2 && NPC.frame.Y != 4)
                        {
                            FlyingFrame();
                            return;
                        }

                        if (Timer < 2)
                        {
                            canDrawShadows = true;
                            ResetAllOldCaches();
                        }

                        UpdateAllOldCaches();

                        const int firstChargeTime = 17;
                        const int secondChargeTime = 9;

                        if (Timer < firstChargeTime)
                        {
                            float factor = Timer / firstChargeTime;
                            shadowScale = Helper.Lerp(2.5f, 1f, factor);
                            shadowAlpha = Helper.Lerp(0f, 1f, factor);
                        }
                        else if (Timer < firstChargeTime + secondChargeTime)
                        {
                            float factor = (Timer - firstChargeTime) / secondChargeTime;
                            shadowScale = Helper.Lerp(1.5f, 1f, factor);
                            shadowAlpha = Helper.Lerp(0f, 1f, factor);
                        }

                        Timer++;
                        if (Timer > firstChargeTime + secondChargeTime)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;

                            shadowScale = 1.2f;
                            Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, NPC.Center, pitch: 0.4f);
                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);

                            if (!VaultUtils.isClient)
                            {
                                NPC.NewProjectileDirectInAI<ExchangePhaseAnmi>(NPC.Center, Vector2.Zero, 1, 0, NPC.target,
                                BurstTime, NPC.whoAmI);
                            }
                        }
                    }
                    break;
                case 1://吼叫并生成电粒子
                    {
                        UpdateAllOldCaches();

                        if (!VaultUtils.isServer)
                        {
                            if (Timer < BurstTime)
                            {
                                float factor = Timer / BurstTime;
                                float length = Helper.Lerp(80, 1400, factor);

                                for (int i = 0; i < 5; i++)
                                {
                                    PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2CircularEdge(length, length),
                                        Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                                }
                            }

                            if (Timer < RoaringTime)
                            {
                                float factor = Timer / RoaringTime;
                                shadowScale = Helper.Lerp(1f, 2.5f, factor);
                                shadowAlpha = Helper.Lerp(1f, 0f, factor);
                                Vector2 pos = NPC.Center + (NPC.rotation.ToRotationVector2() * 60);
                                if ((int)Timer % 10 == 0)
                                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Coralite.ThunderveinYellow, 0.2f);
                                if ((int)Timer % 20 == 0)
                                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.2f);
                            }
                        }

                        Timer++;
                        if (Timer > RoaringTime + 25)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
