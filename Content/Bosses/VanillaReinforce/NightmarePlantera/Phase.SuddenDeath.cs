using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public partial class NightmarePlantera
    {
        public void SuddenDeath()
        {
            switch ((int)SonState)
            {
                default:
                case 0: //逐渐消失
                    {
                        if (Timer % 8 == 0)
                        {
                            Vector2 center = Target.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
                            Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), center, (Target.Center - center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<NightmareSpike>(),
                                   1, 0, NPC.target, 40, -2, 1100);
                            (p.ModProjectile as NightmareSpike).ShootTime = 180;
                            (p.ModProjectile as NightmareSpike).canHitPlayer = false;
                        }

                        Phase2Fade(() =>
                        {
                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return Target.Center + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return Target.Center + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<NightmareBite>(), 999999, 4,NPC.target, ai0: 3, ai1: 60, ai2: -2);
                        });
                    }
                    break;
                case 1:
                    {
                        DoRotation(0.1f);

                        if (Vector2.Distance(NPC.Center, Target.Center) > 220)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.65f;
                            if (speed > 24)
                                speed = 24;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                        {
                            NPC.velocity *= 0.8f;
                        }

                        if (Timer > 55)
                        {
                            useMeleeDamage = false;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2: //咬完了之后的后摇阶段
                    {
                        NPC.velocity *= 0.9f;

                        if (Timer == 7)
                        {
                            SoundStyle st = CoraliteSoundID.BottleExplosion_Item107;
                            st.Pitch = 1f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 15)
                        {
                            DoRotation(0.04f);
                        }

                        if (Timer > 30)
                        {
                            ResetStates();
                        }
                    }
                    break;
            }

            Timer++;
        }
    }
}
