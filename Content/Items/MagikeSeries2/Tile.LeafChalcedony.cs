using Coralite.Content.Items.Glistent;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class LeafChalcedony : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LeafChalcedonyTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<Chalcedony>()
                .AddIngredient<LeafStone>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
