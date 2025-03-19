using Coralite.Content.Tiles.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9AlloyTileItem():BasePlaceableItem(Item.sellPrice(0,0,20),ItemRarityID.LightRed
        ,ModContent.TileType<B9AlloyTile>(),AssetDirectory.SteelItems)
    {
        public override void AddRecipes()
        {
            CreateRecipe(20)
                .AddIngredient<B9Alloy>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
