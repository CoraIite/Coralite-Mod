using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeDoor : BaseDoorItem
    {
        public RedJadeDoor() : base(Item.sellPrice(0, 0, 10), ItemRarityID.White, ModContent.TileType<RedJadeDoorClosed>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(3)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
