using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberBathtub : BasePlaceableItem
    {
        public GelFiberBathtub() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberBathtubTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(12)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
