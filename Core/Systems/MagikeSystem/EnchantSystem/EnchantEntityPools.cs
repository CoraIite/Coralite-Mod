using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class EnchantEntityPools : ModSystem
    {
        public static EnchantEntityPool accessoryPool;
        public static EnchantEntityPool armorPool;
        public static EnchantEntityPool weaponPool;

        public static EnchantEntityPool remodelableWeaponPool;

        public override void Load()
        {
            weaponPool = new EnchantEntityPool();
            #region addWeaponPool
            weaponPool
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
                .AddBonus(new OtherEnchant_WeaponAttackBonus(Enchant.Level.Max, 3f, 0, 0))
                //其他暴击加成
                .AddBonus(new OtherEnchant_WeaponCritBonus(Enchant.Level.Five, 4f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponCritBonus(Enchant.Level.Max, 8f, 0, 0))
                //其他击退加成
                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(Enchant.Level.Four, 5f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(Enchant.Level.Five, 10f, 0, 0))
                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(Enchant.Level.Max, 15f, 0, 0))
                //其他攻速加成
                .AddBonus(new OtherEnchant_UseSpeedBonus(Enchant.Level.Four, 3f, 0, 0))
                .AddBonus(new OtherEnchant_UseSpeedBonus(Enchant.Level.Five, 6f, 0, 0))
                .AddBonus(new OtherEnchant_UseSpeedBonus(Enchant.Level.Max, 10f, 0, 0))
                //其他魔法消耗减少加成
                .AddBonus(new OtherEnchant_ManaCostBonus(Enchant.Level.Four, 3f, 0, 0))
                .AddBonus(new OtherEnchant_ManaCostBonus(Enchant.Level.Five, 6f, 0, 0))
                .AddBonus(new OtherEnchant_ManaCostBonus(Enchant.Level.Max, 10f, 0, 0))
                //其他武器大小
                .AddBonus(new OtherEnchant_ItemScaleBonus(Enchant.Level.Four, 5f, 0, 0))
                .AddBonus(new OtherEnchant_ItemScaleBonus(Enchant.Level.Five, 10f, 0, 0))
                .AddBonus(new OtherEnchant_ItemScaleBonus(Enchant.Level.Max, 15f, 0, 0))

                //特殊攻击加成
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.One, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.Two, 1f, 0, 0))
                .AddBonus(new SpecialEnchant_WeaponAttackBonus(Enchant.Level.Three, 1f, 0, 0))
                //射出额外射弹
                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Four, ProjectileID.WandOfSparkingSpark, 0.2f, 0))
                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Four, ProjectileID.Bullet, 0.2f, 0))
                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Five, ProjectileID.Fireball, 0.3f, 0))
                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Five, ProjectileID.ThunderSpearShot, 0.3f, 0))
                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Five, ProjectileID.ThunderStaffShot, 0.3f, 0))

                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Max, ProjectileID.ThunderSpearShot, 0.4f, 0))
                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Max, ProjectileID.ThunderStaffShot, 0.4f, 0))
                .AddBonus(new SpecialEnchant_ShootExtraBonus(Enchant.Level.Max, ProjectileID.WandOfFrostingFrost, 0.3f, 0));
            #endregion

            remodelableWeaponPool = weaponPool.Clone();
            remodelableWeaponPool.FindAndRemoveAll<SpecialEnchant_ShootExtraBonus>(2, Enchant.Level.Max);
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
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Four, 1f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorAttackBonus(Enchant.Level.Max, 2f, 0, 0))

                .AddBonus(new OtherEnchant_ArmorCritBonus(Enchant.Level.Four, 1f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorCritBonus(Enchant.Level.Five, 2f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorCritBonus(Enchant.Level.Max, 3f, 0, 0))

                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(Enchant.Level.Four, 2f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(Enchant.Level.Five, 4f, 0, 0))
                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(Enchant.Level.Max, 6f, 0, 0))

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
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Two, 2f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Three, 3f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Four, 4f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Five, 5f, 0, 0))
                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(Enchant.Level.Max, 6f, 0, 0))

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
            weaponPool = null;
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
