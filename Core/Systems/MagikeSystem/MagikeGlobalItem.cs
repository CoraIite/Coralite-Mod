using Coralite.Content.Items.Materials;
using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem
{
    public class MagikeItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => true;
        public override bool InstancePerEntity => true;

        /// <summary>
        /// 物品中存储的魔能最大值
        /// </summary>
        public int magikeMax;
        /// <summary>
        /// 物品中当前存储的魔能量
        /// </summary>
        public int magike;

        /// <summary>
        /// 物品自身的魔能含量，设置了这个就能让物品被普通透镜转化成魔能
        /// </summary>
        public int magiteAmount = -1;

        public bool accessoryOrArmorCanEnchant;

        public int magike_CraftRequired = -1;
        public int stack_CraftRequired;
        public IMagikeCraftCondition condition = null;

        private Enchant enchant;
        public Enchant Enchant
        {
            get
            {
                enchant ??= new Enchant();
                return enchant;
            }
            set
            {
                enchant = value;
            }
        }

        public override GlobalItem Clone(Item from, Item to)
        {
            if (to.TryGetGlobalItem(out MagikeItem mItem) && from.TryGetGlobalItem(out MagikeItem fromItem))
            {
                mItem.Enchant = fromItem.enchant;
            }
            return base.Clone(from, to);
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            base.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            base.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
        }

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

        public override void SaveData(Item item, TagCompound tag)
        {
            if (enchant != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (enchant.datas[i] != null)
                    {
                        string main = "Enchant" + i.ToString();
                        tag.Add(main + "_name", enchant.datas[i].GetType().FullName);
                        tag.Add(main + "_level", (int)enchant.datas[i].level);
                        tag.Add(main + "_bonus0", enchant.datas[i].bonus0);
                        tag.Add(main + "_bonus1", enchant.datas[i].bonus1);
                        tag.Add(main + "_bonus2", enchant.datas[i].bonus2);
                    }
                }
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            for (int i = 0; i < 3; i++)
            {
                string main = "Enchant" + i.ToString();
                if (tag.ContainsKey(main + "_name"))
                {
                    try
                    {
                        var t = Type.GetType(tag.GetString(main + "_name"));
                        if (t is null)
                            continue;

                        Enchant.Level level = (Enchant.Level)tag.GetInt(main + "_level");
                        float bonus0 = tag.GetFloat(main + "_bonus0");
                        float bonus1 = tag.GetFloat(main + "_bonus1");
                        float bonus2 = tag.GetFloat(main + "_bonus2");
                        var data = (EnchantData)Activator.CreateInstance(t, level, bonus0, bonus1, bonus2);

                        Enchant.datas[i] = data;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem is IMagikeSenderItem senderItem)
            {
                string sendDelay = string.Concat("每 ", senderItem.SendDelay, " 秒发送一次魔能\n");
                string perSend = $"每次发送魔能量： {senderItem.HowManyPerSend}\n";
                string connectLengthMax = $"连接距离： {senderItem.ConnectLengthMax} 格\n";
                string howManyCanConnect = $"连接数量： {senderItem.HowManyCanConnect}";
                TooltipLine line = new TooltipLine(Mod, "magikeSend", string.Concat(sendDelay, perSend, connectLengthMax, howManyCanConnect));
                tooltips.Add(line);
            }

            if (item.ModItem is IMagikeGeneratorItem generatorItem)
            {
                string GenerateDelay = string.Concat("每 ", generatorItem.GenerateDelay, " 秒生产一次魔能\n");
                string howManyToGenerate = generatorItem.HowManyToGenerate < 0 ? "" : $"每次生产魔能量：{generatorItem.HowManyToGenerate}";
                TooltipLine line = new TooltipLine(Mod, "magikeGenerate", GenerateDelay+howManyToGenerate);
                tooltips.Add(line);
            }

            if (item.ModItem is IMagikeFactoryItem factoryItem)
            {
                string workTimeMax = string.Concat("工作时间：", factoryItem.WorkTimeMax, " 秒\n");
                string workCost = string.Concat("每次工作消耗 ", factoryItem.WorkCost, " 魔能");
                TooltipLine line = new TooltipLine(Mod, "magikeFactory", workTimeMax+ workCost);
                tooltips.Add(line);
            }

            if (enchant != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (enchant.datas[i] != null)
                    {
                        TooltipLine line = new TooltipLine(Mod, "enchant" + i.ToString(), enchant.datas[i].Description);
                        line.OverrideColor = GetColor(enchant.datas[i].level);
                        tooltips.Add(line);
                    }
                }
            }

            if (MagikeSystem.remodelRecipes.ContainsKey(item.type))
                tooltips.Add(new TooltipLine(Mod, "canRemodel", "可重塑"));

            if (magiteAmount > 0)
            {
                string magikeAmount = $"魔能含量: {magiteAmount}";
                TooltipLine line = new TooltipLine(Mod, "magiteAmount", magikeAmount);
                if (magiteAmount < 300)
                    line.OverrideColor = Coralite.Instance.MagicCrystalPink;
                else if (magiteAmount < 1000)
                    line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
                else if (magiteAmount < 2_0000)
                    line.OverrideColor = Coralite.Instance.SplendorMagicoreLightBlue;
                else
                    line.OverrideColor = Color.Orange;

                tooltips.Add(line);
            }

            if (magike_CraftRequired > 0)
            {
                string stackAmount = $"物品需求量： {stack_CraftRequired}\n";
                string magikeAmount = $"消耗魔能： {magike_CraftRequired}";
                string conditionNeed = condition == null ? "" : ("\n" + condition.Description);
                TooltipLine line = new TooltipLine(Mod, "remodelConition", stackAmount + magikeAmount + conditionNeed);
                if (magike_CraftRequired < 300)
                    line.OverrideColor = Coralite.Instance.MagicCrystalPink;
                else if (magike_CraftRequired < 1000)
                    line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
                else if (magiteAmount < 2_0000)
                    line.OverrideColor = Coralite.Instance.SplendorMagicoreLightBlue;
                else
                    line.OverrideColor = Color.Orange;

                tooltips.Add(line);
            }

        }

        private static Color GetColor(Enchant.Level level)
        {
            return level switch
            {
                Enchant.Level.Nothing => Color.Gray,
                Enchant.Level.One => Color.White,
                Enchant.Level.Two => Color.CornflowerBlue,
                Enchant.Level.Three => Color.LightSeaGreen,
                Enchant.Level.Four => Color.Pink,
                Enchant.Level.Five => Color.Yellow,
                _ => Color.Orange
            };
        }

        public void Limit()
        {
            magike = Math.Clamp(magike, 0, magikeMax);
        }

        public bool Charge(int howManyMagike)
        {
            bool ChargeOrDischarge = howManyMagike >= 0;
            if (magike >= magikeMax && ChargeOrDischarge)
                return false;

            magike += howManyMagike;
            Limit();

            return true;
        }

        public bool Cosume(int howManyMagike)
        {
            howManyMagike = Math.Abs(howManyMagike);

            if (magike < howManyMagike)
                return false;

            magike += howManyMagike;
            return true;
        }
    }
}
