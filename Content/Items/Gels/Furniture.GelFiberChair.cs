using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberChair:BaseChairItem
    {
        public GelFiberChair() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberChairTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(4)
                .AddTile(TileID.Solidifier)
                .Register();
        }

    }
}
