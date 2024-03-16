using Coralite.Content.Tiles.Gel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberDoor : BaseDoorItem
    {
        public GelFiberDoor() : base(0, ItemRarityID.White, ModContent.TileType<GelFiberDoorClosedTile>(), AssetDirectory.GelItems)
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
