using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;
using Terraria.Audio;
using Coralite.Core;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void Split()
        {
            switch ((int)SonState)
            {
                case 0: //先要落地才行
                    Jump(2f, 8f, onLanding: () => SonState++);
                    break;
                case 1: //弹弹
                    Scale = Vector2.Lerp(Scale, new Vector2(1.25f, 0.8f), 0.1f);
                    int width = (int)(NPC.width * Scale.X * 0.7f);
                    int height2 = (int)(NPC.height * Scale.Y * 0.7f);
                    for (int i = 0; i < 6; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(width, height2), DustID.TintableDust,
                            Helpers.Helper.NextVec2Dir() * Main.rand.NextFloat(0.2f, 1f), 150, new Color(78, 136, 255, 80), 2f);
                        dust.noGravity = true;
                        dust.velocity *= 0.5f;
                    }

                    if (Scale.X > 1.2f)
                    {
                        SonState++;
                    }
                    break;
                case 2:
                    Scale = Vector2.Lerp(Scale, new Vector2(0.8f, 1.25f), 0.1f);
                    int width3 = (int)(NPC.width * Scale.X * 0.7f);
                    int height4 = (int)(NPC.height * Scale.Y * 0.7f);
                    for (int i = 0; i < 6; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(width3, height4), DustID.TintableDust,
                            Helpers.Helper.NextVec2Dir() * Main.rand.NextFloat(0.2f, 1f), 150, new Color(78, 136, 255, 80), 2f);
                        dust.noGravity = true;
                        dust.velocity *= 0.5f;
                    }

                    if (Scale.Y > 1.2f)
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient) //分裂成好几个
                        {
                            int howMany = Helpers.Helper.ScaleValueForDiffMode(1, 1, 2, 3);
                            for (int i = 0; i < howMany; i++)
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SlimeAvatar>(), Target: NPC.target);
                        }
                        SonState++;
                    }
                    break;
                case 3:
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.1f);
                    if (Math.Abs(Scale.Y - 1) < 0.05f)
                    {
                        SonState++;
                    }
                    break;
                case 4:
                    Jump(2f, 8f, onLanding: () => SonState++);
                    break;
                default:
                case 5:
                    ResetStates();
                    break;
            }
        }
    }
}
