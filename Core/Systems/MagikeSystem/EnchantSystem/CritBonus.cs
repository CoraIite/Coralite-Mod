using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_WeaponCritBonus : OtherEnchant
    {
        public OtherEnchant_WeaponCritBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            crit += bonus0;
        }

        public override string Description => $"暴击率 +{(int)bonus0}%";
    }

    public class OtherEnchant_ArmorCritBonus : OtherEnchant
    {
        public OtherEnchant_ArmorCritBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance(DamageClass.Generic) += bonus0;
        }

        public override string Description => $"暴击率 +{(int)bonus0}%";
    }

    public class OtherEnchant_AccessoryCritBonus : OtherEnchant
    {
        public OtherEnchant_AccessoryCritBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += bonus0;
        }

        public override string Description => $"暴击率 +{(int)bonus0}%";
    }
}
