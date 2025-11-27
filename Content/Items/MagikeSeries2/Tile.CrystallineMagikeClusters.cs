using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineMagikeClusters() : BasePlaceableItem(0, ModContent.RarityType<CrystallineMagikeRarity>(), ModContent.TileType<CrystallineMagikeClustersTile>(), AssetDirectory.MagikeSeries2Item)
        , IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<CrystallineMagike, CrystallineMagikeClusters>(MagikeHelper.CalculateMagikeCost<BrilliantLevel>( 12, 60 * 5), 60)
                .AddIngredient<Skarn>(80)
                .AddIngredient<MagicalPowder>(8)
                .Register();
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.ReplaceResult<CrystallineMagike>(40);
            recipe.AddIngredient<CrystallineMagikeClusters>()
                .AddTile<SkarnCutterTile>()
                .DisableDecraft()
                .Register();
        }
    }
}
