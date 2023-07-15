using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_WeaponKnockbackBonus : OtherEnchant
    {
        public OtherEnchant_WeaponKnockbackBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            knockback += bonus0 / 100f;
        }

        public override string Description => $"击退 +{(int)bonus0}%";
    }

    public class OtherEnchant_ArmorKnockbackBonus : OtherEnchant
    {
        public OtherEnchant_ArmorKnockbackBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetKnockback(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"击退 +{(int)bonus0}%";
    }

    public class OtherEnchant_AccessoryKnockbackBonus : OtherEnchant
    {
        public OtherEnchant_AccessoryKnockbackBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetKnockback(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"击退 +{(int)bonus0}%";
    }
}
