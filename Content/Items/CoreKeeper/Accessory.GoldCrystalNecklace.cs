using Coralite.Content.Items.CoreKeeper.Bases;
using Coralite.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class GoldCrystalNecklace : PolishableAccessory
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = RarityType<RareRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient<AncientGemstone>()
                .AddIngredient(ItemID.SoulofMight)
                .AddTile(TileID.TinkerersWorkbench)
                .AddOnCraftCallback(Polish)
                .Register();
        }

        public override void Polish(Recipe recipe, Item item, List<Item> list, Item destinationStack)
        {
            if (item.type == Type)
            {
                ModItem mi = item.ModItem;
                if (mi is GoldCrystalNecklace gcn)
                {
                    gcn.polished = Main.rand.NextBool(10);
                    //gtr.polished = true;
                    if (gcn.polished)
                        item.rare = RarityType<EpicRarity>();
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (polished)
            {
                player.GetDamage(DamageClass.Melee) += 0.123f;
                player.GetDamage(DamageClass.Ranged) += 0.125f;
            }
            else
            {
                player.GetDamage(DamageClass.Melee) += 0.09f;
                player.GetDamage(DamageClass.Ranged) += 0.094f;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (polished)
            {
                TooltipLine tooltip = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "ItemName", null);
                if (tooltip != null)
                {
                    tooltip.Text = this.GetLocalization("Polished", () => "经过抛光的 ").Value + tooltip.Text;
                }

                TooltipLine tooltip2 = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Tooltip0", null);
                if (tooltip2 != null)
                {
                    tooltip2.Text = this.GetLocalization("PolishedToolTip", () => "+12.3%近战伤害\n+12.5%远程伤害").Value;
                }
                tooltip2 = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Tooltip1", null);
                if (tooltip2 != null)
                    tooltips.Remove(tooltip2);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (polished)
                Item.rare = RarityType<EpicRarity>();
        }
    }
}
