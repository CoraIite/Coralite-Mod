﻿using Coralite.Content.Items.CoreKeeper.Bases;
using Coralite.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class CoralAmulet : PolishableAccessory
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 1);
            Item.rare = RarityType<UncommonRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Coral, 5)
                .AddIngredient(ItemID.Marble, 10)
                .AddTile(TileID.WorkBenches)
                .AddOnCraftCallback(Polish)
                .Register();
        }

        public override void Polish(Recipe recipe, Item item, List<Item> list, Item destinationStack)
        {
            if (item.type == Type)
            {
                ModItem mi = item.ModItem;
                if (mi is GlowTulipRing gtr)
                {
                    gtr.polished = Main.rand.NextBool(10);
                    //gtr.polished = true;
                    if (gtr.polished)
                        item.rare = RarityType<RareRarity>();
                }
            }
        }

        public override void ClonePolish(Item item)
        {
            item.rare = RarityType<EpicRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float speed = 0.06f;

            if (polished)
            {
                speed = 0.10f;
                player.fishingSkill += 5;
            }

            player.moveSpeed += speed;
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
                    tooltip2.Text = this.GetLocalization("PolishedToolTip").Value;
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
