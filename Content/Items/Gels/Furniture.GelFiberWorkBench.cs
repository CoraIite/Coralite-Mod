using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberWorkBench : BasePlaceableItem
    {
        public GelFiberWorkBench() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberWorkBenchTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(10)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
