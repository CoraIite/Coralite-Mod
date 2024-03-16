using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    internal class GelFiberBookcase : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GelFiberBookcaseTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
