using Coralite.Content.Particles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Coralite.Content.Items.Icicle;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void IciclesFall()
        {
            switch (movePhase)
            {
                case 0:    // 追踪玩家
                    {
                        bool lowerThanTarget = NPC.Center.Y > (Target.Center.Y - 100);
                        float xLength = Math.Abs(NPC.Center.X - Target.Center.X);
                        if (lowerThanTarget || xLength > 500)
                        {
                            SetDirection();
                            if (lowerThanTarget)
                                FlyUp();
                            else
                            {
                                NPC.velocity.Y *= 0.9f;
                                ChangeFrameNormally();
                            }
                            if (xLength > 300)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 8, 0.24f, 0.24f, 0.96f);
                            else
                                NPC.velocity.X *= 0.96f;

                            if (Timer > 300)
                            {
                                ResetStates();
                                return;
                            }

                            break;
                        }

                        movePhase = 1;
                        Timer = 0;
                        NPC.rotation = NPC.direction;
                        SetDirection();
                        SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                        GetMouseCenter(out _, out Vector2 mouseCenter2);
                        Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                        Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                        for (int i = 0; i < 4; i++)
                            IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                            {
                                return NPC.Center + (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30;
                            }, 16);
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:     //从下至上射出冰锥
                    {
                        ChangeFrameNormally();
                        NPC.velocity *= 0.97f;
                        if (Timer < 20)
                            break;

                        if (Timer < 100)
                        {
                            float factor = Timer / 80;
                            NPC.rotation = NPC.rotation.AngleLerp(NPC.direction * (1f - factor * 1.3f), 0.06f);

                            GetMouseCenter(out Vector2 targetDir, out Vector2 mouseCenter);
                            if (Timer % 2 == 0)
                                Particle.NewParticle(mouseCenter, targetDir.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * 4, CoraliteContent.ParticleType<IceFog>(), Color.White, 0.8f);

                            if (Timer % 12 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), mouseCenter, targetDir * 12, ModContent.ProjectileType<IcicleProj_Hostile>(), 13, 8f);
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                            }
                            break;
                        }

                        movePhase = 2;
                        NPC.frame.Y = 2;
                        NPC.velocity *= 0;
                        Timer = 0;
                    }
                    break;
                case 2:     //吼叫并在玩家头顶射出冰锥
                    {
                        NPC.rotation = NPC.rotation.AngleLerp(0, 0.14f);
                        if (Timer < 20)
                            break;

                        if (Timer == 20)
                        {
                            NPC.frame.Y = 0;
                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                            PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                            Main.instance.CameraModifiers.Add(modifier);
                        }

                        if (Timer % 6 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Target.Center + new Vector2(Main.rand.Next(-100, 100), -500),
                                Vector2.Zero, ModContent.ProjectileType<IcicleFalling_Hostile>(), 13, 8f);
                            projectile.velocity = (Target.Center + Main.rand.NextVector2Circular(40, 40) - projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                            projectile.netUpdate = true;
                        }

                        if (Timer < 80)
                        {
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            if (Timer % 10 == 0)
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                            if (Timer % 20 == 0)
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);

                            break;
                        }

                        HaveARest(30);
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
