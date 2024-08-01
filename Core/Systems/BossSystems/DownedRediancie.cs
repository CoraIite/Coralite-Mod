using Coralite.Content.Tiles.RedJades;
using Coralite.Content.WorldGeneration;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownRediancie()
        {
            //如果已经击败过赤玉灵，那么就不会运行
            if (downedRediancie)
                return;

            //写一行字
            string key = "赤色的矿物出现了";
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(key), Coralite.RedJadeRed);
            else if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), Coralite.RedJadeRed);

            //生成赤玉矿物
            WorldGenHelper.GenerateOre(ModContent.TileType<RedJadeTile>(), 0.00010, 0.3f, 0.45f, (int x, int y) => { return Main.tile[x, y].TileType == TileID.Dirt || Main.tile[x, y].TileType == TileID.Stone; });

            downedRediancie = true;
        }

    }
}
