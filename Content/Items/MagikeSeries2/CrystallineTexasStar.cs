using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineTexasStar() : BasePlaceableItem(Item.sellPrice(), ItemRarityID.Yellow
        , ModContent.TileType<CrystallineTexasStarTile>(), AssetDirectory.MagikeSeries2Item), IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<CrystallineLemna, CrystallineTexasStar>(MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 10), 3)
                .Register();
        }
    }
}
