using Coralite.Content.Particles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void HorizontalDash()
        {
            switch (movePhase)
            {
                case 0:
                    {
                        float yLength = Math.Abs(NPC.Center.Y - Target.Center.Y);
                        if (yLength > 32 ||
                            Math.Abs(NPC.Center.X - Target.Center.X) > 400)
                        {
                            SetDirection();
                            NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                            if (yLength > 32)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.2f, 0.2f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 8f, 0.25f, 0.25f, 0.96f);
                            ChangeFrameNormally();

                            if (Timer > 400)
                            {
                                ResetStates();
                                return;
                            }

                            break;
                        }

                        //前往冲刺攻击
                        SetDirection();
                        Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<Flash_WithOutLine>(), Coralite.Instance.IcicleCyan, 0.8f);
                        SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                        movePhase = 1;
                        NPC.frame.Y = 0;
                        NPC.velocity *= 0;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    {
                        if (Timer == 20)
                        {
                            NPC.velocity.Y = 0f;
                            NPC.velocity.X = NPC.direction * 18f;
                            canDrawShadows = true;
                            InitCaches();
                            SetDirection();
                        }

                        if (Timer < 30)
                            break;

                        if (Timer < 85)
                        {
                            //if (Timer % 10 == 0)
                            //    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction, 0) * 10f, ModContent.ProjectileType<IceBreath>(), 30, 5f);
                            if (NPC.direction * (NPC.Center.X - Target.Center.X) > 300)
                                Timer = 85;
                            break;
                        }

                        if (Timer > 85)
                        {
                            canDrawShadows = false;
                            NPC.velocity *= 0.93f;
                            ChangeFrameNormally();
                            if (Timer > 100)
                                ResetStates();
                            break;
                        }
                    }
                    break;
                default:
                    ResetEffects();
                    break;
            }

            Timer++;
        }
    }
}
