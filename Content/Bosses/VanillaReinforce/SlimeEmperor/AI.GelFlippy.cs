using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void GelFlippy()
        {
            switch ((SonState))
            {
                case 0: //保证自己在地上
                    Jump(2, 8, onLanding: () => SonState++);
                    break;
                case 1: //弹弹
                    ScaleToTarget(1.2f, 0.9f, 0.2f, Scale.X > 1.15f, () => SonState++);
                    break;
                case 2:
                    ScaleToTarget(0.8f, 1.25f, 0.2f, Scale.Y > 1.2f, () =>
                    {
                        SonState++;
                        //召唤飞翼史莱姆
                        int howMany = Helpers.Helper.ScaleValueForDiffMode(1, 1, 2, 2);
                        int damage = Helpers.Helper.ScaleValueForDiffMode(20, 15, 18, 20);
                        for (int i = 0; i < howMany; i++)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<GelFlippy>(), Target: NPC.target);
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, NPC.Center);
                    });
                    break;
                case 3: //小跳，给玩家一定时间反应
                    Jump(1f, 6f, () => SonState++);
                    break;
                case 4:
                    Jump(4f, 6f, () => SonState++);
                    break;
                default:
                case 5:
                    ResetStates();
                    break;
            }
        }
    }
}
