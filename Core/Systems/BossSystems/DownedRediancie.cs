using Coralite.Core.Systems.WorldValueSystem;
using Terraria;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownRediancie()
        {
            DownedRediancie d = ModContent.GetInstance<DownedRediancie>();

            bool b = false;
            NPC.SetEventFlagCleared(ref b, -1);

            if (!d.Value)
                d.SetAndSync(true);

            //写一行字
            //string key = "赤色的矿物出现了";
            //if (Main.netMode == NetmodeID.SinglePlayer)
            //    Main.NewText(Language.GetTextValue(key), Coralite.RedJadeRed);
            //else if (VaultUtils.isServer)
            //    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), Coralite.RedJadeRed);

            //生成赤玉矿物
            //WorldGenHelper.GenerateOre(ModContent.TileType<RedJadeTile>(), 0.00010, 0.3f, 0.45f, (int x, int y) => { return Main.tile[x, y].TileType == TileID.Dirt || Main.tile[x, y].TileType == TileID.Stone; });

        }
    }

    public class DownedRediancie : WorldFlag { }
}
