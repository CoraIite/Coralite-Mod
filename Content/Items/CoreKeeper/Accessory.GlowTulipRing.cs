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
    public class GlowTulipRing : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public bool polished;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = RarityType<UncommonRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowTulip)
                .AddIngredient(ItemID.IronBar, 10)
                .AddTile(TileID.WorkBenches)
                .AddOnCraftCallback(Polish)
                .Register();
        }

        public void Polish(Recipe recipe, Item item, List<Item> list, Item destinationStack)
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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float factor = polished ? 0.8f : 0.6f;
            Lighting.AddLight(player.Center, new Microsoft.Xna.Framework.Vector3(0.55f, 0.95f, 0.8f) * factor);
            if (polished)
            {
                player.lifeRegen += 2;
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
                if (tooltip2 != null )
                {
                    tooltip2.Text = this.GetLocalization("PolishedToolTip", () => "+4亮光\n每秒+1.0点生命值").Value;
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (polished)
                tag.Add("Polished",polished);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("Polished",out bool p))
                polished = p;

            if (polished)
            {
                Item.rare = RarityType<RareRarity>();
            }
        }
    }
}
