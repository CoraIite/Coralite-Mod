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
                        if (Math.Abs(NPC.Center.Y - Target.Center.Y) > 30 ||
                            Math.Abs(NPC.Center.X - Target.Center.X) > 260)
                        {
                            SetDirection();
                            NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 2f, 0.08f, 0.08f, 0.96f);
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
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    {
                        if (Timer == 20)
                        {
                            NPC.velocity.Y = 0f;
                            NPC.velocity.X = NPC.direction * 15f;
                            float factor = (NPC.Center.Y - Target.Center.Y) / 100;
                            factor = Math.Clamp(factor, -1, 1);
                            NPC.velocity.RotatedBy(factor * 0.6f);
                        }

                        if (Timer < 85 && NPC.direction * (NPC.Center.X - Target.Center.X) > 260)
                        {
                            Timer = 85;
                            break;
                        }

                        if (Timer > 85)
                        {
                            NPC.velocity *= 0.97f;
                            ChangeFrameNormally();
                            if (Timer > 105)
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
