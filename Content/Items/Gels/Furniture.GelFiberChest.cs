using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberChest : BaseChestItem
    {
        public GelFiberChest() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberChestTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(8)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
