using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberCandle : BaseCandleItem
    {
        public GelFiberCandle() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberCandleTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(2)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
