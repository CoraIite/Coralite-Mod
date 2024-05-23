using Coralite.Content.Items.CoreKeeper.Bases;
using Coralite.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class GoldCrystalRing : PolishableAccessory
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = RarityType<UncommonRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient<AncientGemstone>()
                .AddTile(TileID.TinkerersWorkbench)
                .AddOnCraftCallback(Polish)
                .Register();
        }

        public override void Polish(Recipe recipe, Item item, List<Item> list, Item destinationStack)
        {
            if (item.type == Type)
            {
                ModItem mi = item.ModItem;
                if (mi is GoldCrystalRing gcr)
                {
                    gcr.polished = Main.rand.NextBool(10);
                    //gtr.polished = true;
                    if (gcr.polished)
                        item.rare = RarityType<EpicRarity>();
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (polished)
            {
                player.GetDamage(DamageClass.Ranged) += 0.089f;
                player.GetAttackSpeed(DamageClass.Ranged) += 0.04f;
            }
            else
            {
                player.GetDamage(DamageClass.Ranged) += 0.063f;
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
