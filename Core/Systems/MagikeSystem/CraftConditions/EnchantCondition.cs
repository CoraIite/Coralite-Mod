using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
using System;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem.CraftConditions
{
    public class EnchantCondition : IMagikeCraftCondition
    {
        private static readonly Lazy<EnchantCondition> singleton = new Lazy<EnchantCondition>(() => new EnchantCondition());
        public static EnchantCondition Instance { get => singleton.Value; }

        public string Description => Language.GetOrRegister($"Mods.Coralite.MagikeSystem.Conditions.EnchantCondition", () => "拥有特殊注魔时可进行魔能合成").Value;

        public bool CanCraft(Item item)
        {
            if (item.TryGetGlobalItem(out MagikeItem magikeItem))
                if (magikeItem.Enchant.datas[2] is not null && magikeItem.Enchant.datas[2] is SpecialEnchant_RemodelableBonus)
                    return true;

            return false;
        }

    }
}
