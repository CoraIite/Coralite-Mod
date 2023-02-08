using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RedJadeWorkBench : BaseWorkBenchItem
    {
        public RedJadeWorkBench() : base(Item.sellPrice(0, 0, 5), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeWorkBench>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(2)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
