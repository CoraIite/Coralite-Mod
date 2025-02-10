using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeItem
    {
        //只是为了判断是否为null才用的这个
        internal MagikeEnchant enchant;
        public MagikeEnchant Enchant
        {
            get
            {
                enchant ??= new MagikeEnchant();
                return enchant;
            }
            set
            {
                enchant = value;
            }
        }

        /// <summary>
        /// 内部使用，表示当前注魔的百分比，获取该值请使用<see cref="EnchantPercent"/>
        /// </summary>
        internal float enchantPercent;

        /// <summary>
        /// 当前注魔的百分比<br></br>
        /// 为 0 到 100 之间的值
        /// </summary>
        public float EnchantPercent { get => enchantPercent; }
        /// <summary>
        /// 注魔已满
        /// </summary>
        public bool PerfactEnchant => enchantPercent >= 100;

        /// <summary>
        /// 增加注魔的进度
        /// </summary>
        /// <param name="percent"></param>
        public void AddEnchantPercent(float percent)
        {
            enchantPercent += percent;
            enchantPercent = Math.Clamp(enchantPercent, 0, 100);
        }

        //private void SaveEnchant(TagCompound tag)
        //{
        //    if (enchant != null)
        //    {
        //        for (int i = 0; i < 3; i++)
        //        {
        //            if (enchant.datas[i] != null)
        //            {
        //                string main = "Enchant" + i.ToString();
        //                tag.Add(main + "_name", enchant.datas[i].GetType().FullName);
        //                tag.Add(main + "_level", (int)enchant.datas[i].level);
        //                tag.Add(main + "_bonus0", enchant.datas[i].bonus0);
        //                tag.Add(main + "_bonus1", enchant.datas[i].bonus1);
        //                tag.Add(main + "_bonus2", enchant.datas[i].bonus2);
        //            }
        //        }
        //    }
        //}

        //private void LoadEnchant(TagCompound tag)
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        string main = "Enchant" + i.ToString();
        //        if (tag.ContainsKey(main + "_name"))
        //        {
        //            try
        //            {
        //                var t = Type.GetType(tag.GetString(main + "_name"));
        //                if (t is null)
        //                    continue;

        //                MagikeEnchant.Level level = (MagikeEnchant.Level)tag.GetInt(main + "_level");
        //                float bonus0 = tag.GetFloat(main + "_bonus0");
        //                float bonus1 = tag.GetFloat(main + "_bonus1");
        //                float bonus2 = tag.GetFloat(main + "_bonus2");
        //                var data = (EnchantData)Activator.CreateInstance(t, level, bonus0, bonus1, bonus2);

        //                Enchant.datas[i] = data;
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //        }
        //    }
        //}

        #region  普通加成

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (enchant == null)
                return;

            enchant.datas?[0]?.ModifyWeaponDamage(item, player, ref damage);
            enchant.datas?[1]?.ModifyWeaponDamage(item, player, ref damage);
            enchant.datas?[2]?.ModifyWeaponDamage(item, player, ref damage);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (enchant == null)
                return;

            enchant.datas?[0]?.UpdateEquip(item, player);
            enchant.datas?[1]?.UpdateEquip(item, player);
            enchant.datas?[2]?.UpdateEquip(item, player);
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (enchant == null)
                return;

            enchant.datas?[0]?.UpdateAccessory(item, player, hideVisual);
            enchant.datas?[1]?.UpdateAccessory(item, player, hideVisual);
            enchant.datas?[2]?.UpdateAccessory(item, player, hideVisual);
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            float? f = Enchant?.datas?[1]?.UseSpeedMultiplier(item, player);
            return f ?? 1f;
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            enchant?.datas?[1]?.ModifyManaCost(item, player, ref reduce, ref mult);
        }

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            enchant?.datas?[1]?.ModifyItemScale(item, player, ref scale);
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            enchant?.datas?[1]?.ModifyWeaponCrit(item, player, ref crit);
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            enchant?.datas?[1]?.ModifyWeaponKnockback(item, player, ref knockback);
        }

        #endregion

        #region 特殊加成

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            enchant?.datas?[2]?.ModifyShootStats(item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            enchant?.datas?[2]?.Shoot(item, player, source, position, velocity, type, damage, knockback);
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        #endregion

    }
}
