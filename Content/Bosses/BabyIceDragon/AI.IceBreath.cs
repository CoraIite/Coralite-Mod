using Coralite.Content.Items.Icicle;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void IceBreath()
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
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 8f, 0.18f, 0.2f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Math.Abs(Target.Center.X - NPC.Center.X) > 200)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 16f, 0.3f, 0.25f, 0.96f);
                            else
                                NPC.velocity.X *= 0.98f;
                            ChangeFrameNormally();
                            if (Timer > 400)
                                ResetStates();

                            break;
                        }

                        SetDirection();
                        movePhase = 1;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                case 1:
                    {
                        ChangeFrameNormally();
                        if (Vector2.Distance(NPC.Center, Target.Center) > 200)
                        {
                            SetDirection();
                            NPC.directionY = (Target.Center.Y - 200) > NPC.Center.Y ? 1 : -1;
                            float yLength = Math.Abs(Target.Center.Y - 200 - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 3f, 0.14f, 0.1f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Math.Abs(Target.Center.X - NPC.Center.X) > 160)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.14f, 0.1f, 0.96f);
                            else
                                NPC.velocity.X *= 0.98f;
                        }

                        if (Timer == 10)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                            GetMouseCenter(out _, out Vector2 mouseCenter2);
                            for (int i = 0; i < 4; i++)
                                IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                {
                                    return NPC.Center + (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30;
                                }, 16);
                            Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                            Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                        }

                        if (Timer < 30)
                            break;

                        if (Timer < 41)
                        {
                            //生成冰吐息弹幕
                            if (Timer % 5 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                                Vector2 targetDir = (Target.Center + Main.rand.NextVector2CircularEdge(30, 30) - NPC.Center).SafeNormalize(Vector2.Zero);
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    for (int i = -1; i < 1; i++)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), mouseCenter, targetDir.RotatedBy(i * 0.05f) * 10f, ModContent.ProjectileType<IceBreath>(), 10, 5f);
                            }
                            break;
                        }

                        int restTime = Main.masterMode ? 10 : 30;
                        HaveARest(restTime);
                        return;
                    }
                default:
                    ResetStates();
                    break;
            }

            Timer++;
        }
    }
}
