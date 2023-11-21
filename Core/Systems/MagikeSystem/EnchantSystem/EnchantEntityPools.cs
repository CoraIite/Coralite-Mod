using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class EnchantEntityPools : ModSystem
    {
#pragma warning disable CA2211 // 非常量字段应当不可见
        public static EnchantEntityPool accessoryPool;
        public static EnchantEntityPool armorPool;

        public static EnchantEntityPool weaponPool_Generic;
        public static EnchantEntityPool weaponPool_Melee;
        public static EnchantEntityPool weaponPool_Magic;
        public static EnchantEntityPool weaponPool_Ranged;
        public static EnchantEntityPool weaponPool_Summon;

        public static EnchantEntityPool remodelableWeaponPool;
#pragma warning restore CA2211 // 非常量字段应当不可见

        public override void Load()
        {
            weaponPool_Generic = new EnchantEntityPool();
            #region addWeaponPool
            weaponPool_Generic
                //普通攻击加成
                .AddBonus(new BasicEnchant_WeaponAttackBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new BasicEnchant_WeaponAttackBonus(Enchant.Level.Two, 2f, 0, 0))
                .AddBonus(new BasicEnchant_WeaponAttackBonus(Enchant.Level.Three, 4f, 0, 0))
                .AddBonus(new BasicEnchant_WeaponAttackBonus(Enchant.Level.Four, 6f, 0, 0))
                .AddBonus(new BasicEnchant_WeaponAttackBonus(Enchant.Level.Five, 7f, 0, 0))
                .AddBonus(new BasicEnchant_WeaponAttackBonus(Enchant.Level.Max, 8f, 0, 0))

                //其他攻击加成
                .AddBonus(new OtherEnchant_WeaponAttackBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponAttackBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponAttackBonus(Enchant.Level.Three, 2f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponAttackBonus(Enchant.Level.Four, 2f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponAttackBonus(Enchant.Level.Five, 3f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponAttackBonus(Enchant.Level.Max, 4f, 0, 0))
                //其他暴击加成
                .AddBonus(new OtherEnchant_WeaponCritBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponCritBonus(Enchant.Level.Max, 3f, 0, 0))
                //其他击退加成
                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(Enchant.Level.Four, 5f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(Enchant.Level.Five, 10f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(Enchant.Level.Max, 15f, 0, 0))
                //其他攻速加成
                .AddBonus(new OtherEnchant_UseSpeedBonus(Enchant.Level.Four, 3f, 0, 0))
                .AddBonus(new OtherEnchant_UseSpeedBonus(Enchant.Level.Five, 6f, 0, 0))
                .AddBonus(new OtherEnchant_UseSpeedBonus(Enchant.Level.Max, 8f, 0, 0))
                //其他魔法消耗减少加成
                .AddBonus(new OtherEnchant_ManaCostBonus(Enchant.Level.Four, 3f, 0, 0))
                .AddBonus(new OtherEnchant_ManaCostBonus(Enchant.Level.Five, 6f, 0, 0))
                .AddBonus(new OtherEnchant_ManaCostBonus(Enchant.Level.Max, 8f, 0, 0))
                //其他武器大小
                .AddBonus(new OtherEnchant_ItemScaleBonus(Enchant.Level.Four, 5f, 0, 0))
                .AddBonus(new OtherEnchant_ItemScaleBonus(Enchant.Level.Five, 8f, 0, 0))
                .AddBonus(new OtherEnchant_ItemScaleBonus(Enchant.Level.Max, 10f, 0, 0))

                //特殊攻击加成
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.Three, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.Four, 2f, 0, 0))
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.Five, 2f, 0, 0))
                //植生：叶刃
                .AddBonus(new SpecialEnchant_BotanicusBonus(Enchant.Level.Max, 0, 0, 0))
                //刚：强化防御
                .AddBonus(new SpecialEnchant_MetallonBonus(Enchant.Level.Max, 0, 0, 0))
                //冻：冰块
                .AddBonus(new SpecialEnchant_FreosanBonus(Enchant.Level.Max, 0, 0, 0))
                //灼：火球
                .AddBonus(new SpecialEnchant_HeatanBonus(Enchant.Level.Max, 0, 0, 0))
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.Max, 3f, 0, 0));

            weaponPool_Melee = weaponPool_Generic.Clone();
            weaponPool_Melee.FindAndRemoveAll<OtherEnchant_ManaCostBonus>(1);

            weaponPool_Ranged = weaponPool_Generic.Clone();
            weaponPool_Ranged.FindAndRemoveAll<OtherEnchant_ManaCostBonus>(1);
            weaponPool_Ranged.FindAndRemoveAll<OtherEnchant_ItemScaleBonus>(1);

            weaponPool_Magic = weaponPool_Generic.Clone();
            weaponPool_Magic.FindAndRemoveAll<OtherEnchant_ItemScaleBonus>(1);

            weaponPool_Summon = weaponPool_Generic.Clone();
            weaponPool_Summon.FindAndRemoveAll<OtherEnchant_ManaCostBonus>(1);
            weaponPool_Summon.FindAndRemoveAll<OtherEnchant_WeaponKnockbackBonus>(1);
            weaponPool_Summon.FindAndRemoveAll<OtherEnchant_WeaponCritBonus>(1);

            #endregion

            remodelableWeaponPool = weaponPool_Generic.Clone();
            remodelableWeaponPool.FindAndRemoveAll(2, Enchant.Level.Max);
            remodelableWeaponPool.AddBonus(new SpecialEnchant_RemodelableBonus(Enchant.Level.Max, 0, 0, 0));

            armorPool = new EnchantEntityPool();
            armorPool
                .AddBonus(new BasicEnchant_ArmorDefenceBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new BasicEnchant_ArmorDefenceBonus(Enchant.Level.Two, 2f, 0, 0))
                .AddBonus(new BasicEnchant_ArmorDefenceBonus(Enchant.Level.Three, 3f, 0, 0))
                .AddBonus(new BasicEnchant_ArmorDefenceBonus(Enchant.Level.Four, 4f, 0, 0))
                .AddBonus(new BasicEnchant_ArmorDefenceBonus(Enchant.Level.Five, 6f, 0, 0))
                .AddBonus(new BasicEnchant_ArmorDefenceBonus(Enchant.Level.Max, 8f, 0, 0))

                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Three, 1f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Four, 2f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Max, 3f, 0, 0))

                .AddBonus(new OtherEnchant_ArmorCritBonus(Enchant.Level.Four, 1f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorCritBonus(Enchant.Level.Five, 1f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorCritBonus(Enchant.Level.Max, 2f, 0, 0))

                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(Enchant.Level.Four, 2f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(Enchant.Level.Five, 3f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(Enchant.Level.Max, 4f, 0, 0))

                .AddBonus(new OtherEnchant_ArmorManaCostBonus(Enchant.Level.Four, 2f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorManaCostBonus(Enchant.Level.Five, 3f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorManaCostBonus(Enchant.Level.Max, 4f, 0, 0))

                .AddBonus(new SpecialEnchant_ArmorDefenceBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_ArmorDefenceBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_ArmorDefenceBonus(Enchant.Level.Three, 1f, 0, 0))

                .AddBonus(new SpecialEnchant_ArmorMoveSpeedBonus(Enchant.Level.Four, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_ArmorMoveSpeedBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new SpecialEnchant_ArmorMoveSpeedBonus(Enchant.Level.Max, 3f, 0, 0));

            accessoryPool = new EnchantEntityPool();
            accessoryPool
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Three, 2f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Four, 2f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Five, 3f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Max, 4f, 0, 0))

                .AddBonus(new OtherEnchant_AccessoryAttackBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryAttackBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryAttackBonus(Enchant.Level.Three, 1f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryAttackBonus(Enchant.Level.Four, 1f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryAttackBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryAttackBonus(Enchant.Level.Max, 2f, 0, 0))

                .AddBonus(new OtherEnchant_AccessoryCritBonus(Enchant.Level.Five, 1f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryCritBonus(Enchant.Level.Max, 2f, 0, 0))

                .AddBonus(new OtherEnchant_AccessoryKnockbackBonus(Enchant.Level.Four, 1f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryKnockbackBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryKnockbackBonus(Enchant.Level.Max, 3f, 0, 0))

                .AddBonus(new OtherEnchant_AccessoryManaCostBonus(Enchant.Level.Five, 1f, 0, 0))
                .AddBonus(new OtherEnchant_AccessoryManaCostBonus(Enchant.Level.Max, 2f, 0, 0))

                .AddBonus(new SpecialEnchant_AccessoryDefenceBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_AccessoryDefenceBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_AccessoryDefenceBonus(Enchant.Level.Three, 1f, 0, 0))

                .AddBonus(new SpecialEnchant_AccessoryMoveSpeedBonus(Enchant.Level.Four, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_AccessoryMoveSpeedBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new SpecialEnchant_AccessoryMoveSpeedBonus(Enchant.Level.Max, 3f, 0, 0));
        }

        public override void Unload()
        {
            weaponPool_Generic = null;
            weaponPool_Summon = null;
            weaponPool_Magic = null;
            weaponPool_Melee = null;
            weaponPool_Ranged = null;

            armorPool = null;
            accessoryPool = null;

            remodelableWeaponPool = null;
        }

        public static bool TryGetSlecialEnchantPool(Item item, out EnchantEntityPool pool)
        {
            switch (item.type)
            {
                default:
                    break;
                case ItemID.SkyFracture:
                    pool = remodelableWeaponPool;
                    return true;
            }

            pool = null;
            return false;
        }
    }
}
