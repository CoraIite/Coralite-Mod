//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader.Core;

//namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
//{
//    public class EnchantEntityPools : ModSystem
//    {
//#pragma warning disable CA2211 // 非常量字段应当不可见
//        public static Dictionary<int, EnchantEntityPool> SpecialEnchantPools;

//        public static EnchantEntityPool accessoryPool;
//        public static EnchantEntityPool armorPool;

//        public static EnchantEntityPool weaponPool_Generic;
//        public static EnchantEntityPool weaponPool_Melee;
//        public static EnchantEntityPool weaponPool_Magic;
//        public static EnchantEntityPool weaponPool_Ranged;
//        public static EnchantEntityPool weaponPool_Summon;

//        public static EnchantEntityPool remodelableWeaponPool;
//#pragma warning restore CA2211 // 非常量字段应当不可见

//        public override void Load()
//        {
//            #region 初始化各种注魔池
//            weaponPool_Generic = new EnchantEntityPool();
//            #region addWeaponPool
//            weaponPool_Generic
//                //普通攻击加成
//                .AddBonus(new BasicEnchant_WeaponAttackBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new BasicEnchant_WeaponAttackBonus(MagikeEnchant.Level.Two, 2f, 0, 0))
//                .AddBonus(new BasicEnchant_WeaponAttackBonus(MagikeEnchant.Level.Three, 4f, 0, 0))
//                .AddBonus(new BasicEnchant_WeaponAttackBonus(MagikeEnchant.Level.Four, 6f, 0, 0))
//                .AddBonus(new BasicEnchant_WeaponAttackBonus(MagikeEnchant.Level.Five, 7f, 0, 0))
//                .AddBonus(new BasicEnchant_WeaponAttackBonus(MagikeEnchant.Level.Max, 8f, 0, 0))

//                //其他攻击加成
//                .AddBonus(new OtherEnchant_WeaponAttackBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponAttackBonus(MagikeEnchant.Level.Two, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponAttackBonus(MagikeEnchant.Level.Three, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponAttackBonus(MagikeEnchant.Level.Four, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponAttackBonus(MagikeEnchant.Level.Five, 3f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponAttackBonus(MagikeEnchant.Level.Max, 4f, 0, 0))
//                //其他暴击加成
//                .AddBonus(new OtherEnchant_WeaponCritBonus(MagikeEnchant.Level.Five, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponCritBonus(MagikeEnchant.Level.Max, 3f, 0, 0))
//                //其他击退加成
//                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(MagikeEnchant.Level.Four, 5f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(MagikeEnchant.Level.Five, 10f, 0, 0))
//                .AddBonus(new OtherEnchant_WeaponKnockbackBonus(MagikeEnchant.Level.Max, 15f, 0, 0))
//                //其他攻速加成
//                .AddBonus(new OtherEnchant_UseSpeedBonus(MagikeEnchant.Level.Four, 3f, 0, 0))
//                .AddBonus(new OtherEnchant_UseSpeedBonus(MagikeEnchant.Level.Five, 6f, 0, 0))
//                .AddBonus(new OtherEnchant_UseSpeedBonus(MagikeEnchant.Level.Max, 8f, 0, 0))
//                //其他魔法消耗减少加成
//                .AddBonus(new OtherEnchant_ManaCostBonus(MagikeEnchant.Level.Four, 3f, 0, 0))
//                .AddBonus(new OtherEnchant_ManaCostBonus(MagikeEnchant.Level.Five, 6f, 0, 0))
//                .AddBonus(new OtherEnchant_ManaCostBonus(MagikeEnchant.Level.Max, 8f, 0, 0))
//                //其他武器大小
//                .AddBonus(new OtherEnchant_ItemScaleBonus(MagikeEnchant.Level.Four, 5f, 0, 0))
//                .AddBonus(new OtherEnchant_ItemScaleBonus(MagikeEnchant.Level.Five, 8f, 0, 0))
//                .AddBonus(new OtherEnchant_ItemScaleBonus(MagikeEnchant.Level.Max, 10f, 0, 0))

//                //特殊攻击加成
//                .AddBonus(new SpecialEnchant_WeaponAttackBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_WeaponAttackBonus(MagikeEnchant.Level.Two, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_WeaponAttackBonus(MagikeEnchant.Level.Three, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_WeaponAttackBonus(MagikeEnchant.Level.Four, 2f, 0, 0))
//                .AddBonus(new SpecialEnchant_WeaponAttackBonus(MagikeEnchant.Level.Five, 2f, 0, 0))
//                //植生：叶刃
//                .AddBonus(new SpecialEnchant_BotanicusBonus(MagikeEnchant.Level.Max, 0, 0, 0))
//                //刚：强化防御
//                .AddBonus(new SpecialEnchant_MetallonBonus(MagikeEnchant.Level.Max, 0, 0, 0))
//                //冻：冰块
//                .AddBonus(new SpecialEnchant_FreosanBonus(MagikeEnchant.Level.Max, 0, 0, 0))
//                //灼：火球
//                .AddBonus(new SpecialEnchant_HeatanBonus(MagikeEnchant.Level.Max, 0, 0, 0))
//                .AddBonus(new SpecialEnchant_WeaponAttackBonus(MagikeEnchant.Level.Max, 3f, 0, 0));

//            weaponPool_Melee = weaponPool_Generic.Clone();
//            weaponPool_Melee.FindAndRemoveAll<OtherEnchant_ManaCostBonus>(1);

//            weaponPool_Ranged = weaponPool_Generic.Clone();
//            weaponPool_Ranged.FindAndRemoveAll<OtherEnchant_ManaCostBonus>(1);
//            weaponPool_Ranged.FindAndRemoveAll<OtherEnchant_ItemScaleBonus>(1);

//            weaponPool_Magic = weaponPool_Generic.Clone();
//            weaponPool_Magic.FindAndRemoveAll<OtherEnchant_ItemScaleBonus>(1);

//            weaponPool_Summon = weaponPool_Generic.Clone();
//            weaponPool_Summon.FindAndRemoveAll<OtherEnchant_ManaCostBonus>(1);
//            weaponPool_Summon.FindAndRemoveAll<OtherEnchant_WeaponKnockbackBonus>(1);
//            weaponPool_Summon.FindAndRemoveAll<OtherEnchant_WeaponCritBonus>(1);

//            #endregion

//            remodelableWeaponPool = weaponPool_Generic.Clone();
//            remodelableWeaponPool.FindAndRemoveAll(2, MagikeEnchant.Level.Max);
//            remodelableWeaponPool.AddBonus(new SpecialEnchant_RemodelableBonus(MagikeEnchant.Level.Max, 0, 0, 0));

//            armorPool = new EnchantEntityPool();
//            armorPool
//                .AddBonus(new BasicEnchant_ArmorDefenceBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new BasicEnchant_ArmorDefenceBonus(MagikeEnchant.Level.Two, 2f, 0, 0))
//                .AddBonus(new BasicEnchant_ArmorDefenceBonus(MagikeEnchant.Level.Three, 3f, 0, 0))
//                .AddBonus(new BasicEnchant_ArmorDefenceBonus(MagikeEnchant.Level.Four, 4f, 0, 0))
//                .AddBonus(new BasicEnchant_ArmorDefenceBonus(MagikeEnchant.Level.Five, 6f, 0, 0))
//                .AddBonus(new BasicEnchant_ArmorDefenceBonus(MagikeEnchant.Level.Max, 8f, 0, 0))

//                .AddBonus(new OtherEnchant_ArmorAttackBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorAttackBonus(MagikeEnchant.Level.Two, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorAttackBonus(MagikeEnchant.Level.Three, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorAttackBonus(MagikeEnchant.Level.Four, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorAttackBonus(MagikeEnchant.Level.Five, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorAttackBonus(MagikeEnchant.Level.Max, 3f, 0, 0))

//                .AddBonus(new OtherEnchant_ArmorCritBonus(MagikeEnchant.Level.Four, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorCritBonus(MagikeEnchant.Level.Five, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorCritBonus(MagikeEnchant.Level.Max, 2f, 0, 0))

//                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(MagikeEnchant.Level.Four, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(MagikeEnchant.Level.Five, 3f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorKnockbackBonus(MagikeEnchant.Level.Max, 4f, 0, 0))

//                .AddBonus(new OtherEnchant_ArmorManaCostBonus(MagikeEnchant.Level.Four, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorManaCostBonus(MagikeEnchant.Level.Five, 3f, 0, 0))
//                .AddBonus(new OtherEnchant_ArmorManaCostBonus(MagikeEnchant.Level.Max, 4f, 0, 0))

//                .AddBonus(new SpecialEnchant_ArmorDefenceBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_ArmorDefenceBonus(MagikeEnchant.Level.Two, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_ArmorDefenceBonus(MagikeEnchant.Level.Three, 1f, 0, 0))

//                .AddBonus(new SpecialEnchant_ArmorMoveSpeedBonus(MagikeEnchant.Level.Four, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_ArmorMoveSpeedBonus(MagikeEnchant.Level.Five, 2f, 0, 0))
//                .AddBonus(new SpecialEnchant_ArmorMoveSpeedBonus(MagikeEnchant.Level.Max, 3f, 0, 0));

//            accessoryPool = new EnchantEntityPool();
//            accessoryPool
//                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.Two, 1f, 0, 0))
//                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.Three, 2f, 0, 0))
//                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.Four, 2f, 0, 0))
//                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.Five, 3f, 0, 0))
//                .AddBonus(new BasicEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.Max, 4f, 0, 0))

//                .AddBonus(new OtherEnchant_AccessoryAttackBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryAttackBonus(MagikeEnchant.Level.Two, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryAttackBonus(MagikeEnchant.Level.Three, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryAttackBonus(MagikeEnchant.Level.Four, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryAttackBonus(MagikeEnchant.Level.Five, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryAttackBonus(MagikeEnchant.Level.Max, 2f, 0, 0))

//                .AddBonus(new OtherEnchant_AccessoryCritBonus(MagikeEnchant.Level.Five, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryCritBonus(MagikeEnchant.Level.Max, 2f, 0, 0))

//                .AddBonus(new OtherEnchant_AccessoryKnockbackBonus(MagikeEnchant.Level.Four, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryKnockbackBonus(MagikeEnchant.Level.Five, 2f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryKnockbackBonus(MagikeEnchant.Level.Max, 3f, 0, 0))

//                .AddBonus(new OtherEnchant_AccessoryManaCostBonus(MagikeEnchant.Level.Five, 1f, 0, 0))
//                .AddBonus(new OtherEnchant_AccessoryManaCostBonus(MagikeEnchant.Level.Max, 2f, 0, 0))

//                .AddBonus(new SpecialEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.One, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.Two, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_AccessoryDefenceBonus(MagikeEnchant.Level.Three, 1f, 0, 0))

//                .AddBonus(new SpecialEnchant_AccessoryMoveSpeedBonus(MagikeEnchant.Level.Four, 1f, 0, 0))
//                .AddBonus(new SpecialEnchant_AccessoryMoveSpeedBonus(MagikeEnchant.Level.Five, 2f, 0, 0))
//                .AddBonus(new SpecialEnchant_AccessoryMoveSpeedBonus(MagikeEnchant.Level.Max, 3f, 0, 0));

//            #endregion
//        }

//        public override void PostAddRecipes()
//        {
//            if (Main.dedServ)
//                return;

//            Mod Mod = Coralite.Instance;
//            SpecialEnchantPools = new Dictionary<int, EnchantEntityPool>();

//            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))  //添加魔能重塑合成表
//            {
//                if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(ISpecialEnchantable)))
//                {
//                    ISpecialEnchantable sp = Activator.CreateInstance(t) as ISpecialEnchantable;
//                    SpecialEnchantPools.Add(sp.SelfType, sp.GetEntityPool());
//                }
//            }

//            //跨模组添加重塑合成表
//            foreach (var mod in ModLoader.Mods)
//                if (mod is ICoralite)
//                    foreach (Type t in AssemblyManager.GetLoadableTypes(mod.Code))  //添加魔能重塑合成表
//                    {
//                        if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(ISpecialEnchantable)))
//                        {
//                            ISpecialEnchantable sp = Activator.CreateInstance(t) as ISpecialEnchantable;
//                            SpecialEnchantPools.Add(sp.SelfType, sp.GetEntityPool());
//                        }
//                    }
//        }

//        public override void Unload()
//        {
//            weaponPool_Generic = null;
//            weaponPool_Summon = null;
//            weaponPool_Magic = null;
//            weaponPool_Melee = null;
//            weaponPool_Ranged = null;

//            armorPool = null;
//            accessoryPool = null;

//            remodelableWeaponPool = null;
//        }

//        public static bool TryGetSlecialEnchantPool(Item item, out EnchantEntityPool pool)
//        {
//            switch (item.type)
//            {
//                default:
//                    break;
//                case ItemID.SkyFracture:
//                    pool = remodelableWeaponPool;
//                    return true;
//            }

//            if (SpecialEnchantPools != null && SpecialEnchantPools.ContainsKey(item.type))
//            {
//                pool = SpecialEnchantPools[item.type];
//                return true;
//            }

//            pool = null;
//            return false;
//        }
//    }
//}
