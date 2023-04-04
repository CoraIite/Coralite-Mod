using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeTable : BaseTableItem
    {
        public RedJadeTable() : base(Item.sellPrice(0, 0, 10), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeTable>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(4)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
