using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystalClusters() : BasePlaceableItem(0, ModContent.RarityType<MagicCrystalRarity>(), ModContent.TileType<MagicCrystalClustersTile>(), AssetDirectory.MagikeSeries1Item)
    {
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.ReplaceResult<MagicCrystal>(40);
            recipe.AddIngredient<MagicCrystalClusters>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
