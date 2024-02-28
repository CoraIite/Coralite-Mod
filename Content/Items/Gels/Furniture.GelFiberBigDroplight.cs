using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberBigDroplight:BaseDroplightItem
    {
        public GelFiberBigDroplight() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberBigDroplightTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(5)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.Solidifier)
                .Register();
        }

    }
}
