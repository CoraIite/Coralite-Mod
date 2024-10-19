using Coralite.Content.Walls.Magike;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SmoothSkarnWallItem : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<SmoothSkarnWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<SmoothSkarn>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
