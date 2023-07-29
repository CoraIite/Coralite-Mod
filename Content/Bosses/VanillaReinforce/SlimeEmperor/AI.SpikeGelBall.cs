using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void SpikeGelBall()
        {
            switch ((int)SonState)
            {
                case 0: //确保自己在地上
                    Jump(2f, 8f, onLanding: () => SonState++);
                    break;
                case 1: //变扁
                    Scale = Vector2.Lerp(Scale, new Vector2(1.2f, 0.9f), 0.2f);
                    if (Scale.X > 1.15f)
                    {
                        SonState++;
                        //射刺球弹幕
                        int howMany = Helpers.Helper.ScaleValueForDiffMode(2, 2, 3, 4);
                        int damage = Helpers.Helper.ScaleValueForDiffMode(20, 15, 12, 15);

                        for (int i = 0; i < howMany; i++)
                        {
                            Vector2 vel = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 10;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 3, NPC.height / 3), vel, ModContent.ProjectileType<SpikeGelBall>(),
                                damage, 4f, NPC.target);
                        }
                    }
                    break;
                case 2:
                    Scale = Vector2.Lerp(Scale, new Vector2(0.8f, 1.25f), 0.2f);
                    if (Scale.Y>1.2f)
                    {
                        SonState++;
                    }
                    break;
                case 3:
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.2f);
                    if (Math.Abs(Scale.X-1)<0.05f)
                    {
                        Scale = Vector2.One;
                        SonState++;
                    }
                    break;
                default:
                    ResetStates();
                    break;
            }
        }
    }
}
