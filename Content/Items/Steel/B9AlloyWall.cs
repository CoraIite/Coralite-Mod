using Coralite.Content.Walls;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9AlloyWallItem : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<B9AlloyWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<B9AlloyTileItem>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
