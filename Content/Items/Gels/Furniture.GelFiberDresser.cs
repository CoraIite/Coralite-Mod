using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberDresser : BasePlaceableItem
    {
        public GelFiberDresser() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberDresserTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(16)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
