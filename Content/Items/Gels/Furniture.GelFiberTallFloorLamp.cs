using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberTallFloorLamp:BaseFloorLampItem
    {
        public GelFiberTallFloorLamp() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberTallFloorLampTile>(), AssetDirectory.GelItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(3)
                .AddIngredient(ItemID.Torch, 2)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
