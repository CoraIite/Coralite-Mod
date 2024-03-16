using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    internal class GeiFiberDroplight : BaseDroplightItem
    {
        public GeiFiberDroplight() : base(0, ItemRarityID.White, ModContent.TileType<GeiFiberDroplightTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Solidifier)
                .Register();
        }

    }
}
