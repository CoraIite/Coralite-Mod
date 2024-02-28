using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberToilet : BaseToiletItem
    {
        public GelFiberToilet() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberToiletTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(6)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}