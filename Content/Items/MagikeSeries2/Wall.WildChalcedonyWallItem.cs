using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Content.Walls.Magike;
using Coralite.Core;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class WildChalcedonyWallItem : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<WildChalcedonyWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<Chalcedony>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }
    }
}
