using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class HardBasalt : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardBasaltTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(2)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
