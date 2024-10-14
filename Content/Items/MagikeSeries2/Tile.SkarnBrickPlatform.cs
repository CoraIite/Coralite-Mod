using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnBrickPlatform : BasePlaceableItem
    {
        public SkarnBrickPlatform() : base(0, ItemRarityID.White, ModContent.TileType<SkarnBrickPlatformTile>(), AssetDirectory.MagikeSeries2Item)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<LeafChalcedony>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
