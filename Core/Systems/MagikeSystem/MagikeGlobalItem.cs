using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
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
        public Enchant Enchant => enchant;

        public void StartEnchant()
        {
            enchant ??= new Enchant();
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return base.UseSpeedMultiplier(item, player);
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(item, player, ref reduce, ref mult);
        }

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            base.ModifyItemScale(item, player, ref scale);
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            base.ModifyWeaponCrit(item, player, ref crit);
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            base.ModifyWeaponKnockback(item, player, ref knockback);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            Enchant?.datas?[0]?.ModifyWeaponDamage(item, player, ref damage);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
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
