using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnBrick : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SkarnBrickTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Skarn>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
