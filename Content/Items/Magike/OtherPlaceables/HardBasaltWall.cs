using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class HardBasaltWall : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Walls.Magike.HardBasaltWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<HardBasalt>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
