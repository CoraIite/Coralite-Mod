using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Coralite.Content.Items.RedJades
{
    public class RedJadeToilet : BaseToiletItem
    {
        public RedJadeToilet() : base(Item.sellPrice(0, 0, 3), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeToilet>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(3)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
