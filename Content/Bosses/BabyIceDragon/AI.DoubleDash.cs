using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void DoubleDash()
        {
            switch (movePhase)
            {
                case 0:
                    {
                        float yLength = Math.Abs(NPC.Center.Y - Target.Center.Y);
                        float xLength = Math.Abs(NPC.Center.X - Target.Center.X);
                        if (yLength > 32 ||
                            xLength > 450)
                        {
                            SetDirection();
                            NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                            if (yLength > 32)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 8f, 0.2f, 0.6f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (xLength > 400)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 14f, 0.25f, 0.6f, 0.96f);
                            else
                                NPC.velocity.X *= 0.93f;

                            NormallyFlyingFrame();

                            if (Timer > 400)
                            {
                                ResetStates();
                                return;
                            }

                            break;
                        }

                        //前往冲刺攻击
                        SetDirection();

                        if (!VaultUtils.isServer)
                        {
                            PRTLoader.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.IcicleCyan, 1.2f);
                        }
                        
                        SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                        movePhase = 1;
                        NPC.velocity *= 0;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    {
                        NormallyFlyingFrame(1);
                        if ((int)Timer == 10)
                        {
                            NPC.velocity.Y = 0f;
                            NPC.velocity.X = NPC.direction * 18f;
                            canDrawShadows = true;
                            InitCaches();
                        }

                        if (Timer < 35)
                            break;

                        if (Timer < 75)
                        {
                            //if (Timer % 10 == 0)
                            //    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction, 0) * 10f, ModContent.ProjectileType<IceBreath>(), 30, 5f);
                            if (Math.Abs(NPC.Center.X - Target.Center.X) > 180)
                                Timer = 75;
                            break;
                        }

                        if (Timer > 75)
                        {
                            canDrawShadows = false;
                            NPC.velocity *= 0.93f;
                            if (Timer > 85)
                            {
                                NPC.velocity *= 0;
                                SetDirection();
                                float angle = (Target.Center - NPC.Center).ToRotation();

                                //角度小于一定值的时候才会冲刺，否则吐息
                                if (angle < -5 * MathHelper.Pi / 6 || angle > 5 * MathHelper.Pi / 6 || (angle > -MathHelper.Pi / 6 && angle < MathHelper.Pi / 6))
                                    movePhase = 3;
                                else
                                    movePhase = 2;
                                Timer = 0;
                            }
                            break;
                        }
                    }
                    break;
                case 2: //强化吐息
                    {
                        NormallyFlyingFrame(1);
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

                        if ((int)Timer == 10)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);

                            if (!VaultUtils.isServer)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter2);
                                for (int i = 0; i < 4; i++)
                                    IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                    {
                                        return NPC.Center + ((NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30);
                                    }, 16);
                                PRTLoader.NewParticle<IceBurstHalo_Reverse>(mouseCenter2, Vector2.Zero, Scale: 0.8f);
                                PRTLoader.NewParticle<IceBurstHalo_Reverse>(mouseCenter2, Vector2.Zero, Scale: 1.2f);
                            }
                        }

                        if (Timer < 30)
                            break;

                        if (Timer < 61)
                        {
                            //生成冰吐息弹幕
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            Vector2 targetDir = (Target.Center + Main.rand.NextVector2CircularEdge(30, 30) - NPC.Center).SafeNormalize(Vector2.Zero);

                            if ((int)Timer % 10 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                                
                                if (!VaultUtils.isClient)
                                {
                                    for (int i = -1; i < 1; i++)
                                    {
                                        int damage = Helper.GetProjDamage(40, 65, 90);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), mouseCenter, targetDir.RotatedBy(i * 0.02f) * 10f, ModContent.ProjectileType<IceBreath>(), damage, 5f);
                                    }
                                }
                            }

                            if (!VaultUtils.isClient && Timer % 15 == 0)
                            {
                                int damage = Helper.GetProjDamage(40, 65, 90);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), mouseCenter, targetDir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 12, ModContent.ProjectileType<IcicleProj_Hostile>(), damage, 8f);
                            }

                            break;
                        }

                        int restTime = Main.masterMode ? 10 : 30;
                        HaveARest(restTime);
                        return;
                    }
                case 3: //强化冲刺
                    {
                        if (Timer < 2)
                        {
                            SetDirection();
                            DoubleDashAngle = (Target.Center - NPC.Center).ToRotation();
                            DoubleDashLength = NPC.Distance(Target.Center) + 100;
                            DoubleDashLength = Math.Clamp(DoubleDashLength, 120, 400);
                            SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);

                            if (!VaultUtils.isServer)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter2);
                                for (int i = 0; i < 4; i++)
                                    IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                    {
                                        return NPC.Center + ((NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30);
                                    }, 16);
                                PRTLoader.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                                PRTLoader.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                            }
                        }

                        if (Timer < 15)
                        {
                            NormallyFlyingFrame();
                            break;
                        }

                        if ((int)Timer == 15)
                        {
                            NPC.velocity = DoubleDashAngle.ToRotationVector2() * DoubleDashLength / 20;
                            canDrawShadows = true;
                            InitCaches();
                        }

                        if (Timer < 35)
                        {
                            if (!VaultUtils.isServer && Timer % 2 == 0)
                                PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), -NPC.velocity * Main.rand.NextFloat(0.2f, 0.5f),
                                    CoraliteContent.ParticleType<SpeedLine>(), Coralite.IcicleCyan, Main.rand.NextFloat(0.1f, 0.4f));

                            NormallyFlyingFrame(1);
                            break;
                        }

                        if (Timer > 35)
                        {
                            NormallyFlyingFrame();
                            canDrawShadows = false;
                            NPC.velocity *= 0.97f;
                            if (Timer > 55)
                            {
                                NPC.velocity *= 0;
                                Timer = 0;
                                SetDirection();
                                ResetStates();
                            }
                        }
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
