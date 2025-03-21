using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineSeaOatsSeed() : BasePlaceableItem(Item.sellPrice(0, 0, 50), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<CrystallineSeaOatsMother>(), AssetDirectory.MagikeSeries2Item)
    {
    }
}
