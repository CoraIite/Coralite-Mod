using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeChest : BaseChestItem
    {
        public RedJadeChest() : base(Item.sellPrice(0, 0, 1), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeChest>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(4)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
