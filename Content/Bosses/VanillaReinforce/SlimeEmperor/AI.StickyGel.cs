﻿using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void StickyGel()
        {
            switch ((int)SonState)
            {
                case 0: //确保自己在地上
                    Jump(2f, 8f, onLanding: () => SonState++);
                    break;
                case 1:
                    ScaleToTarget(1.2f, 0.9f, 0.05f, Scale.X > 1.15f, () =>
                    {
                        SonState++;
                    });
                    break;
                case 2:
                    ScaleToTarget(0.8f, 1.25f, 0.2f, Scale.Y > 1.2f, () =>
                    {
                        SonState++;
                        //射刺球弹幕
                        int howMany = Helper.ScaleValueForDiffMode(2, 2, 2, 4);
                        int damage = Helper.GetProjDamage(75, 95, 115);

                        Vector2 targetDir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < howMany; i++)
                        {
                            Vector2 vel = -targetDir.RotatedBy(Main.rand.NextFloat(-2f, 2f)) * 8;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 3, NPC.height / 3), vel, ModContent.ProjectileType<StickyGel>(),
                                damage, 4f, NPC.target);
                        }
                    });
                    break;
                case 3:
                    ScaleToTarget(1f, 1f, 0.05f, Math.Abs(Scale.X - 1) < 0.05f, () =>
                    {
                        Scale = Vector2.One;
                        SonState++;
                    });
                    break;
                default:
                case 4:
                    ResetStates();
                    break;
            }
        }
    }
}
