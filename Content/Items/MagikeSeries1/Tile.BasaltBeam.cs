using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class BasaltBeam() : BasePlaceableItem(0, ModContent.RarityType<MagicCrystalRarity>(), ModContent.TileType<BasaltBeamTile>(), AssetDirectory.MagikeSeries1Item)
    {
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<Basalt>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
