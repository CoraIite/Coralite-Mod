using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using System;
using Terraria;
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
                            PRTLoader.NewParticle<Sparkle_Big>(NPC.Center, Vector2.Zero, Coralite.IcicleCyan, 1.2f);
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
                            SetDirection();
                        }

                        if (Timer < 35)
                            break;

                        if (Timer < 75)
                        {
                            //if (Timer % 10 == 0)
                            //    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.direction, 0) * 10f, ModContent.ProjectileType<IceBreath>(), 30, 5f);
                            if (Math.Abs(NPC.Center.X - Target.Center.X) > 240)
                                Timer = 75;
                            break;
                        }

                        if (Timer > 75)
                        {
                            canDrawShadows = false;
                            NPC.velocity *= 0.93f;
                            if (Timer > 85)
                                ResetStates();
                            break;
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
