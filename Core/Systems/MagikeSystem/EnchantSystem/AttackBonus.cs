//using Terraria;

//namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
//{
//    public class BasicEnchant_WeaponAttackBonus : BasicEnchant
//    {
//        public BasicEnchant_WeaponAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
//        {
//            damage += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }

//    public class OtherEnchant_WeaponAttackBonus : OtherEnchant
//    {
//        public OtherEnchant_WeaponAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
//        {
//            damage += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }

//    public class SpecialEnchant_WeaponAttackBonus : SpecialEnchant
//    {
//        public SpecialEnchant_WeaponAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
//        {
//            damage += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }


//    public class BasicEnchant_ArmorAttackBonus : BasicEnchant
//    {
//        public BasicEnchant_ArmorAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void UpdateEquip(Item item, Player player)
//        {
//            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }

//    public class OtherEnchant_ArmorAttackBonus : OtherEnchant
//    {
//        public OtherEnchant_ArmorAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void UpdateEquip(Item item, Player player)
//        {
//            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }

//    public class SpecialEnchant_ArmorAttackBonus : SpecialEnchant
//    {
//        public SpecialEnchant_ArmorAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void UpdateEquip(Item item, Player player)
//        {
//            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }


//    public class BasicEnchant_AccessoryAttackBonus : BasicEnchant
//    {
//        public BasicEnchant_AccessoryAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
//        {
//            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }

//    public class OtherEnchant_AccessoryAttackBonus : OtherEnchant
//    {
//        public OtherEnchant_AccessoryAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
//        {
//            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }

//    public class SpecialEnchant_AccessoryAttackBonus : SpecialEnchant
//    {
//        public SpecialEnchant_AccessoryAttackBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
//        {
//            player.GetDamage(DamageClass.Generic) += bonus0 / 100f;
//        }

//        public override string Description => $"攻击力 +{(int)bonus0}%";
//    }
//}
