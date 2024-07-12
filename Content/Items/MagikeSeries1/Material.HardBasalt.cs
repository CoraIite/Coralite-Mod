using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class HardBasalt : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardBasaltTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(4)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
