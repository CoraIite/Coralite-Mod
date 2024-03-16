using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberBed : BasePlaceableItem
    {
        public GelFiberBed() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberBedTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(12)
                .AddIngredient(ItemID.Silk, 12)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
