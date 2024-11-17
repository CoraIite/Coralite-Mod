using Coralite.Content.Particles;
using Coralite.Core;
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
        public void IceTornado()
        {
            switch (movePhase)
            {
                case 0:     //飞向玩家
                    {
                        if (Vector2.Distance(NPC.Center, Target.Center) > 600)
                        {
                            SetDirection();
                            NormallyFlyingFrame();

                            NPC.directionY = (Target.Center.Y - 200) > NPC.Center.Y ? 1 : -1;
                            float yLength = Math.Abs(Target.Center.Y - 200 - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 8f, 0.18f, 0.6f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Math.Abs(Target.Center.X - NPC.Center.X) > 160)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 14f, 0.2f, 0.6f, 0.96f);
                            else
                                NPC.velocity.X *= 0.98f;
                            if (Timer > 400)
                                ResetStates();

                            break;
                        }

                        SetDirection();
                        movePhase = 1;
                        Timer = 0;

                        if (!VaultUtils.isServer)
                        {
                            PRTLoader.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.IcicleCyan, 0.8f);
                        }

                        SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                        NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                        NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f);
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:     //准备动作
                    {
                        NormallyFlyingFrame(changeRot: false);

                        if (Timer < 2)
                        {
                            NPC.velocity = Vector2.UnitY * 6;      //2Pi / 20
                        }
                        if (Timer < 30)
                        {
                            SetDirection();
                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / 30);
                            NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f);
                            break;
                        }

                        if (Timer < 60)
                        {
                            SetDirection();
                            NPC.velocity = -(Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 2;
                            NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                            break;
                        }

                        Helper.PlayPitched("Icicle/Wind" + Main.rand.Next(1, 3).ToString(), 0.2f, 0f, NPC.Center);
                        movePhase = 2;
                        SetDirection();
                        Timer = 0;
                        NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX).RotatedBy(0.9f) * 12;
                        NPC.dontTakeDamage = true;
                        NPC.netUpdate = true;
                    }
                    break;
                case 2:         //龙卷风
                    {
                        if (Timer < 200)
                        {
                            NormallyFlyingFrame(1, false);
                            float distance = Vector2.Distance(Target.Center, NPC.Center);

                            if (distance < 160)
                                NPC.velocity += Vector2.Normalize(Target.Center - NPC.Center) * 1.35f;
                            else
                                NPC.velocity += Vector2.Normalize(Target.Center - NPC.Center) * 0.75f;

                            if (NPC.velocity.Length() > 12)
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 12;

                            NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f);

                            if ((int)Timer % 20 == 0)
                                Helper.PlayPitched("Icicle/Wind" + Main.rand.Next(1, 3).ToString(), 0.2f, 0f, NPC.Center);

                            if ((int)Timer % 8 == 0 && Main.netMode != NetmodeID.MultiplayerClient)     //生成透明弹幕
                            {
                                int damage = Helper.GetProjDamage(40, 65, 100);
                                Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + (NPC.velocity * 6), NPC.velocity * 0.2f, ModContent.ProjectileType<IceTornado>(), damage, 10f);
                                projectile.timeLeft = 40;
                                projectile.netUpdate = true;
                            }

                            //生成龙卷风粒子
                            if (!VaultUtils.isServer)
                            {
                                if ((int)Timer % 2 == 0)
                                {
                                    Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(32, 32), DustID.FrostStaff, -NPC.velocity * 0.3f, Scale: Main.rand.NextFloat(1.8f, 2f));
                                    dust.noGravity = true;
                                }

                                Color tornadoColor = Main.rand.Next(3) switch
                                {
                                    0 => new Color(217, 248, 255, 200),
                                    1 => new Color(120, 211, 231, 200),
                                    _ => new Color(252, 255, 255, 200)
                                };
                                Tornado.Spawn(NPC.Center + (NPC.velocity * 8), NPC.velocity * 0.05f, tornadoColor, 60, NPC.velocity.ToRotation(), Main.rand.NextFloat(0.5f, 0.6f));
                            }

                            break;
                        }

                        if (Timer < 210)
                        {
                            NormallyFlyingFrame();
                            NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                            NPC.dontTakeDamage = false;
                            NPC.velocity *= 0.98f;
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