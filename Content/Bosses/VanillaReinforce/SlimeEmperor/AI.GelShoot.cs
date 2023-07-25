using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void GelShoot()
        {
            switch ((int)SonState)
            {
                default:
                case 0:  //小跳并向玩家发射弹弹凝胶球
                case 1:
                    {
                        Jump(6f, 8f, () =>
                        {
                            SonState++;
                            NPC.TargetClosest();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int sign = Math.Sign(NPC.Center.X - Target.Center.X);
                                int howMany = Helpers.Helper.ScaleValueForDiffMode(2, 2, 3, 4);
                                int damage= Helpers.Helper.ScaleValueForDiffMode(20, 15, 12, 15);
                                SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, NPC.Center);
                                for (int i = 0; i < howMany; i++)
                                {
                                    Vector2 vel = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY).RotatedBy(sign * (0.2f + i * 0.05f))*16;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height), vel, ModContent.ProjectileType<GelBall>(),
                                        damage,4f,NPC.target);
                                }
                            }
                        });
                    }
                    break;
                case 2://重设状态
                    ResetStates();
                    break;
            }
        }
    }
}
