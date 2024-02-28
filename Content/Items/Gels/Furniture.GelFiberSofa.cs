using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberSofa : BasePlaceableItem
    {
        public GelFiberSofa() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberSofaTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(8)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
