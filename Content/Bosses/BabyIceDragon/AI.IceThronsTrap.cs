using Coralite.Content.Particles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria.Graphics.CameraModifiers;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria;
using Coralite.Content.Items.Icicle;
using Terraria.ModLoader;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void IceThornsTrap()
        {
            switch (movePhase)
            {
                case 0:
                    {
                        if (Vector2.Distance(NPC.Center, Target.Center) > 440)
                        {
                            SetDirection();
                            NPC.directionY = (Target.Center.Y - 200) > NPC.Center.Y ? 1 : -1;
                            float yLength = Math.Abs(Target.Center.Y - 200 - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 8f, 0.25f, 0.3f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Math.Abs(Target.Center.X - NPC.Center.X) > 160)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 16f, 0.25f, 0.3f, 0.96f);
                            else
                                NPC.velocity.X *= 0.96f;
                            ChangeFrameNormally();
                            if (Timer > 400)
                                ResetStates();

                            break;
                        }

                        movePhase = 1;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    {
                        do
                        {
                            NPC.velocity *= 0.97f;

                            if ((int)Timer == 30)
                            {
                                NPC.frame.Y = 2;
                                NPC.velocity *= 0;
                            }

                            if (Timer < 50)
                                break;

                            if ((int)Timer == 50)
                            {
                                NPC.frame.Y = 0;
                                SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                                Main.instance.CameraModifiers.Add(modifier);
                            }

                            if ((int)Timer == 60 && Main.netMode != NetmodeID.MultiplayerClient)        //生成冰刺NPC
                            {
                                int howMany = 4;
                                if (Main.expertMode)
                                    howMany = 5;
                                if (Main.masterMode)
                                    howMany = 6;

                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                Vector2 Position = Vector2.Zero;
                                for (int i = 0; i < howMany; i++)
                                {
                                    int randomWidth = Main.rand.Next(240, 350);
                                    Vector2 randomPosition = Main.rand.NextVector2CircularEdge(randomWidth, randomWidth);
                                    for (int k = 0; k < 5; k++)
                                    {
                                        if (Vector2.Distance(randomPosition, Position) > 60)
                                            break;

                                        randomPosition = Main.rand.NextVector2CircularEdge(randomWidth, randomWidth);
                                    }

                                    Position = randomPosition;
                                    NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), Target.Center + randomPosition,
                                        ModContent.NPCType<IceThornsTrap>());
                                    for (int j = 0; j < 2; j++)
                                        IceStarLight.Spawn(mouseCenter,
                                            (npc.Center - NPC.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 10,
                                            1f, () => npc.Center, 16);
                                }
                            }

                            if (Timer < 90)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if ((int)Timer % 10 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                                if ((int)Timer % 20 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);

                                break;
                            }

                            ChangeFrameNormally();

                            if (Timer > 120)
                                ResetStates();

                        } while (false);
                    }
                    break;
                default:
                    ResetStates();
                    break;
            }

            Timer++;
        }
    }
}
