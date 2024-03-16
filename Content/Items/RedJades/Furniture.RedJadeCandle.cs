using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeCandle : BaseCandleItem
    {
        public RedJadeCandle() : base(Item.sellPrice(0, 0, 1), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeCandle>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(1)
                .AddIngredient(ItemID.Torch)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
