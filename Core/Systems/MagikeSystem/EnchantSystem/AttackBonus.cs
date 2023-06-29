using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class BasicEnchant_WeaponAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public BasicEnchant_WeaponAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }

    public class OtherEnchant_WeaponAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public OtherEnchant_WeaponAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }

    public class SpecialEnchant_WeaponAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public SpecialEnchant_WeaponAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }


    public class BasicEnchant_ArmorAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public BasicEnchant_ArmorAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }

    public class OtherEnchant_ArmorAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public OtherEnchant_ArmorAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }

    public class BasicSpecialEnchant_ArmorAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public BasicSpecialEnchant_ArmorAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }


    public class BasicEnchant_AccessoryAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public BasicEnchant_AccessoryAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }

    public class OtherEnchant_AccessoryAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public OtherEnchant_AccessoryAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }

    public class BasicSpecialEnchant_AccessoryAttackBonus : BasicBonusEnchant
    {
        private readonly float bonus;

        public BasicSpecialEnchant_AccessoryAttackBonus(int level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += bonus;
        }

        public override string Description => $"攻击力 +{(int)(bonus * 100)}%";
    }
}
