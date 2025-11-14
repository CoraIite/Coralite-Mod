using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 此物品是否为工具（镐，斧，锤）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsTool(this Item item)
            => item.pick > 0 || item.axe > 0 || item.hammer > 0;

        /// <summary>
        /// 生成被物块破坏所产生的物品，带有同步功能
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static int SpawnItemTileBreakNet(Point16 origin, int itemType)
        {
            int index = Item.NewItem(new EntitySource_TileBreak(origin.X, origin.Y), origin.ToWorldCoordinates()
                , itemType);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index, 1f);

            return index;
        }

        /// <summary>
        /// 生成被物块破坏所产生的物品，带有同步功能
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static int SpawnItemTileBreakNet<T>(Point16 origin)
            where T : ModItem
        {
            int index = Item.NewItem(new EntitySource_TileBreak(origin.X, origin.Y), origin.ToWorldCoordinates()
                , ModContent.ItemType<T>());

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index, 1f);

            return index;
        }
    }
}
