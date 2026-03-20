using Coralite.Content.Walls.Magike;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class BasaltBrickWallItem : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BasaltBrickWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<BasaltBrick>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
