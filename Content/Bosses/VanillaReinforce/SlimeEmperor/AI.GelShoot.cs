using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

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
                        Jump(2f, 6f, () =>
                        {
                            SonState++;
                            NPC.TargetClosest();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int sign = Math.Sign(NPC.Center.X - Target.Center.X);
                                int directlyHowMany = Helpers.Helper.ScaleValueForDiffMode(1, 1, 3, 4);
                                int damage = Helper.GetProjDamage(50, 60, 90);
                                SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, NPC.Center);
                                for (int i = 0; i < directlyHowMany; i++)
                                {
                                    Vector2 vel = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY).RotatedBy(sign * (1f + (i * 0.3f))) * 12;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 3, NPC.height / 3), vel, ModContent.ProjectileType<GelBall>(),
                                        damage, 4f, NPC.target);
                                }

                                directlyHowMany = directlyHowMany > 1 ? directlyHowMany - 1 : 1;
                                for (int i = 0; i < directlyHowMany; i++)
                                {
                                    Vector2 vel = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 16;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 3, NPC.height / 3), vel, ModContent.ProjectileType<GelBall>(),
                                        damage, 4f, NPC.target);
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
