using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_WeaponCritBonus : OtherBonusEnchant
    {
        public OtherEnchant_WeaponCritBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            crit += bonus0;
        }

        public override string Description => $"暴击率 +{(int)bonus0}%";
    }

    public class OtherEnchant_ArmorCritBonus : OtherBonusEnchant
    {
        public OtherEnchant_ArmorCritBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) += bonus0;
        }

        public override string Description => $"暴击率 +{(int)bonus0}%";
    }

    public class OtherEnchant_AccessoryCritBonus : OtherBonusEnchant
    {
        public OtherEnchant_AccessoryCritBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += bonus0;
        }

        public override string Description => $"暴击率 +{(int)bonus0}%";
    }
}
