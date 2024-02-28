using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

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
                .AddIngredient<GelFiber>(12)
                .AddIngredient(ItemID.WaterBucket, 6)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
