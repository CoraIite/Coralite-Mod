using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;

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
