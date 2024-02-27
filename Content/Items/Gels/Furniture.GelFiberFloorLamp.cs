using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberFloorLamp:BaseFloorLampItem
    {
        public GelFiberFloorLamp() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberFloorLampTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(3)
                .AddIngredient(ItemID.Torch,2)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
