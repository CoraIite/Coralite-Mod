using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class EnchantCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<EnchantCondition> singleton = new Lazy<EnchantCondition>(() => new EnchantCondition());
        public static EnchantCondition Instance { get => singleton.Value; }

        public string Description => "拥有特殊注魔时可重塑";

        public bool CanCraft(Item item)
        {
            if (item.TryGetGlobalItem(out MagikeItem magikeItem))
                if (magikeItem.Enchant.datas[2] is not null && magikeItem.Enchant.datas[2] is SpecialEnchant_RemodelableBonus)
                    return true;

            return false;
        }

    }
}
