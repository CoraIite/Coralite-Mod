using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
using Humanizer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem
{
    public class MagikeItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => true;
        public override bool InstancePerEntity => true;

        public int magiteAmount = -1;

        public int magikeRemodelRequired = -1;
        public int stackRemodelRequired;
        public IMagikeRemodelCondition condition = null;

        private Enchant enchant;
        public Enchant Enchant
        {
            get
            {
                enchant ??= new Enchant();
                return enchant;
            }
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



        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (MagikeSystem.remodelRecipes.ContainsKey(item.type))
                tooltips.Add(new TooltipLine(Mod, "canRemodel", "可重塑"));

            if (magiteAmount>0)
            {
                string magikeAmount = $"魔能含量: {magiteAmount}";
                TooltipLine line = new TooltipLine(Mod, "magiteAmount", magikeAmount);
                if (magiteAmount < 300)
                    line.OverrideColor = Coralite.Instance.MagicCrystalPink;
                else if (magiteAmount < 1000)
                    line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
                tooltips.Add(line);
            }

            if (magikeRemodelRequired > 0)
            {
                string stackAmount = $"物品需求量： {stackRemodelRequired}\n";
                string magikeAmount = $"消耗魔能： {magikeRemodelRequired}";
                string conditionNeed = condition == null ? "" : ("\n" + condition.Description);
                TooltipLine line = new TooltipLine(Mod, "remodelConition", stackAmount+magikeAmount+conditionNeed);
                if (magikeRemodelRequired < 300)
                    line.OverrideColor = Coralite.Instance.MagicCrystalPink;
                else if (magikeRemodelRequired < 1000)
                    line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
                //else if (true)
                //{

                //}
                tooltips.Add(line);
            }
        }
    }
}
