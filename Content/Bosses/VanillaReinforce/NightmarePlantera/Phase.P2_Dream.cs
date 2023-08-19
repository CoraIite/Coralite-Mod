using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera
    {
        public void Dream_Phase2()
        {
            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;

            switch ((int)State)
            {
                default:
                case (int)AIStates.nightmareBite:
                    NightmareBite();
                    break;

            }
        }

        public void NightmareBite()
        {
            switch ((int)SonState)
            {
                default:
                case 0: //逐渐消失
                    {
                        if (alpha>0)
                        {
                            alpha -= 0.05f;
                            if (alpha < 0)
                                alpha = 0;
                        }

                        if (Timer>30)
                        {
                            SonState++;
                            alpha = 1;

                            if (Math.Abs(Target.velocity.X) < 0.1f&& Math.Abs(Target.velocity.Y) < 0.1f)
                            {
                                NPC.Center = Target.Center + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            }
                            else
                                NPC.Center = Target.Center + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                            int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<NightmareBite>(), damage, 4, ai0: 0, ai1: 60, ai2: -1);
                            Timer = 0;
                            NPC.rotation = (Target.Center - NPC.Center).ToRotation();

                        }
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

                        if (Timer>60)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2: //咬完了之后的后摇阶段
                    {
                        NPC.velocity *= 0.75f;
                        if (Timer > 40)
                        {
                            SetPhase2States();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void SetPhase2States()
        {
            Timer = 0;
            SonState = 0;
        }

        /// <summary>
        /// 梦境阶段的战斗的重设状态
        /// </summary>
        public void SetPhase2DreamingStates()
        {

        }
    }
}
