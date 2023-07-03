using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class BasicEnchant_WeaponAttackBonus : BasicBonusEnchant
    {
        public BasicEnchant_WeaponAttackBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }

    public class OtherEnchant_WeaponAttackBonus : BasicBonusEnchant
    {
        public OtherEnchant_WeaponAttackBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }

    public class SpecialEnchant_WeaponAttackBonus : BasicBonusEnchant
    {
        public SpecialEnchant_WeaponAttackBonus(Enchant.Level level, float bonus) : base(level) { bonus0 = bonus; }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }


    public class BasicEnchant_ArmorAttackBonus : BasicBonusEnchant
    {
        public BasicEnchant_ArmorAttackBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }

    public class OtherEnchant_ArmorAttackBonus : BasicBonusEnchant
    {
        public OtherEnchant_ArmorAttackBonus(Enchant.Level level, float bonus) : base(level) { bonus0 = bonus; }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";

        public static OtherEnchant_ArmorAttackBonus DeserializeData(TagCompound tag)
        {
            int whichslot = tag.GetInt("whichSlot");
            Enchant.Level level = (Enchant.Level)tag.GetInt("level");
            float bonus0 = tag.GetFloat("_bonus0");
            float bonus1 = tag.GetFloat("_bonus1");
            float bonus2 = tag.GetFloat("_bonus2");

            return new OtherEnchant_ArmorAttackBonus(level, bonus0);
        }
    }

    public class SpecialEnchant_ArmorAttackBonus : BasicBonusEnchant
    {
        public SpecialEnchant_ArmorAttackBonus(Enchant.Level level, float bonus) : base(level) { bonus0 = bonus; }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }


    public class BasicEnchant_AccessoryAttackBonus : BasicBonusEnchant
    {
        public BasicEnchant_AccessoryAttackBonus(Enchant.Level level, float bonus) : base(level) { bonus0 = bonus; }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }

    public class OtherEnchant_AccessoryAttackBonus : BasicBonusEnchant
    {
        public OtherEnchant_AccessoryAttackBonus(Enchant.Level level, float bonus) : base(level) { bonus0 = bonus; }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }

    public class SpecialEnchant_AccessoryAttackBonus : BasicBonusEnchant
    {
        public SpecialEnchant_AccessoryAttackBonus(Enchant.Level level, float bonus) : base(level) { bonus0 = bonus; }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
        }

        public override string Description => $"攻击力 +{(int)bonus0}%";
    }
}
