using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.YujianSystem
{
    public class YujianSource : EntitySource_ItemUse
    {
        public BaseYujian Yujian => Item.ModItem as BaseYujian;
        public YujianSource(Player player, Item item, string context = null) : base(player, item, context)
        {
        }
    }
}
