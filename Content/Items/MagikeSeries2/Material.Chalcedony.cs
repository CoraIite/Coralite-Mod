using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Content.Walls.Magike;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class Chalcedony : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ChalcedonyTile>());
        }
    }

    public class ChalcedonyWallItem : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<ChalcedonyWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<Chalcedony>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
