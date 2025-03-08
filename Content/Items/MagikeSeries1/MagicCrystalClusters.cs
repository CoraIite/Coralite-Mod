using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystalClusters() : BasePlaceableItem(0, ModContent.RarityType<MagicCrystalRarity>(), ModContent.TileType<MagicCrystalClustersTile>(), AssetDirectory.MagikeSeries1Item)
        , IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<MagicCrystal, MagicCrystalClusters>(MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 12, 60 * 5), 60)
                .AddIngredient<Basalt>(80)
                .AddIngredient<MagicalPowder>(8)
                .Register();
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.ReplaceResult<MagicCrystal>(40);
            recipe.AddIngredient<MagicCrystalClusters>()
                .AddTile(TileID.WorkBenches)
                .DisableDecraft()
                .Register();
        }
    }
}
