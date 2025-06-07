using Terraria;

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
    }
}
