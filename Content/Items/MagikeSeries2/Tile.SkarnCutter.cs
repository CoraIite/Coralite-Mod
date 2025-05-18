using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnCutter() : BasePlaceableItem(Item.sellPrice(0, 1), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<SkarnCutterTile>(), AssetDirectory.MagikeSeries2Item), IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<Chalcedony, SkarnCutter>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 3, 30)
                , 20)
                .AddIngredient<SkarnBrick>(5)
                .Register();
        }
    }
}
