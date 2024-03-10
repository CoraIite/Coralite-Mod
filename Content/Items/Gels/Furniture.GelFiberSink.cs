using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberSink : BasePlaceableItem
    {
        public GelFiberSink() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberSinkTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
