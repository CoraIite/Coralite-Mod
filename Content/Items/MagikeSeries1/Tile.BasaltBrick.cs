using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class BasaltBrick() : BasePlaceableItem(0, ModContent.RarityType<MagicCrystalRarity>(), ModContent.TileType<BasaltBrickTile>(), AssetDirectory.MagikeSeries1Item)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
