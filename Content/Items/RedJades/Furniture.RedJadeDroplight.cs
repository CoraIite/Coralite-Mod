using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeDroplight : BaseDroplightItem
    {
        public RedJadeDroplight() : base(Item.sellPrice(0, 0, 1), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeDroplight>(), AssetDirectory.RedJadeItems) { }

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
