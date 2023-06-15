using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Helpers
{
    public static class MagikeHelper
    {
        public static MagikeItem GetMagikeItem(this Item item)
        {
            return item.GetGlobalItem<MagikeItem>();
        }

        /// <summary>
        /// 获取到当前鼠标上的魔能物块并显示魔能物块实体中的魔能量/魔能最大值
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void ShowMagikeNumber(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int x = tile.TileFrameX / 18;
            int y = tile.TileFrameY / 18;
            Point16 position = new Point16(i - x, j - y);

            if (TileEntity.ByPosition.ContainsKey(position) && TileEntity.ByPosition[position] is MagikeContainer magikeContainer)
                    Main.instance.MouseText(magikeContainer.magike + " / " + magikeContainer.magikeMax, 0, 0, -1, -1, -1, -1);
        }
    }
}
