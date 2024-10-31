using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

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
                        if (lowerThanTarget || xLength > 600)
                        {
                            SetDirection();
                            if (lowerThanTarget)
                                FlyUp();
                            else
                            {
                                NPC.velocity.Y *= 0.9f;
                                NormallyFlyingFrame();
                            }
                            if (xLength > 300)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 14f, 0.24f, 0.24f, 0.96f);
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

                        if (!CLUtils.isServer)
                        {
                            GetMouseCenter(out _, out Vector2 mouseCenter2);
                            Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                            Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                            for (int i = 0; i < 4; i++)
                                IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                {
                                    return NPC.Center + ((NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30);
                                }, 16);
                        }
                        
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:     //瞄准玩家射出冰锥
                    {
                        NormallyFlyingFrame(1, false);
                        NPC.velocity *= 0.97f;
                        if (Timer < 20)
                        {
                            NPC.rotation = 0;
                            break;
                        }

                        if (Timer < 100)
                        {
                            float factor = Timer / 80;
                            NPC.rotation = NPC.direction * (Target.Center.Y - NPC.Center.Y) * 0.008f;
                            NPC.rotation = Math.Clamp(NPC.rotation, -0.45f, 0.45f);

                            SetDirection();

                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            Vector2 targetDir = (Target.Center - NPC.Center).SafeNormalize(Vector2.One);

                            if (!CLUtils.isServer && (int)Timer % 2 == 0)
                                Particle.NewParticle(mouseCenter, targetDir.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * 4, CoraliteContent.ParticleType<Fog>(), Color.White, 0.8f);

                            if ((int)Timer % 12 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = Helper.GetProjDamage(40, 65, 90);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), mouseCenter, targetDir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * 12, ModContent.ProjectileType<IcicleProj_Hostile>(), damage, 8f);
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                            }
                            break;
                        }

                        movePhase = 2;
                        NPC.frame.X = 0;
                        NPC.frame.Y = 3;
                        NPC.velocity *= 0;
                        Timer = 0;
                    }
                    break;
                case 2:     //吼叫并在玩家头顶射出冰锥
                    {
                        NPC.rotation = NPC.rotation.AngleLerp(0, 0.14f);
                        if (Timer < 20)
                            break;

                        if ((int)Timer == 20)
                        {
                            NPC.frame.X = 1;
                            NPC.frame.Y = 1;
                            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                            if (!CLUtils.isServer)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                PunchCameraModifier modifier = new(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                                Main.instance.CameraModifiers.Add(modifier);
                            }
                        }

                        if (!CLUtils.isClient && (int)Timer % 6 == 0)
                        {
                            int damage = Helper.GetProjDamage(40, 65, 90);
                            Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Target.Center + new Vector2(Main.rand.Next(-100, 100), -500),
                                Vector2.Zero, ModContent.ProjectileType<IcicleFalling_Hostile>(), damage, 8f);
                            projectile.velocity = (Target.Center + Main.rand.NextVector2Circular(40, 40) - projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                            projectile.netUpdate = true;
                        }

                        if (Timer < 80)
                        {
                            if (!CLUtils.isServer)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if ((int)Timer % 10 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                                if ((int)Timer % 20 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                            }

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
