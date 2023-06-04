using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Botanical
{
    public class IdentifyLoupe : ModItem
    {
        public override string Texture => AssetDirectory.BotanicalItems + Name;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("鉴定镜");

            // Tooltip.SetDefault("消耗20魔力\n放在背包中时可以通过右键未鉴定的植物以查看它们的基因");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 0, 40, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.HellstoneBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
