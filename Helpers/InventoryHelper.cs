using Terraria;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 如果玩家至少有 1 个空物品slot，则返回第一个打开的非弹药或硬币物品slot，否则返回 -1
        /// </summary>
        public static int GetFreeInventorySlot(Player Player)
        {
            for (int k = 0; k < 49; k++)
            {
                Item Item = Player.inventory[k];
                if (Item is null || Item.IsAir)
                    return k;
            }

            return -1;
        }
    }
}
