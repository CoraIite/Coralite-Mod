using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineFountainItem() : BasePlaceableItem(Item.buyPrice(0,4), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<CrystallineFountain>(), AssetDirectory.MagikeSeries2Item)
    {
    }
}
