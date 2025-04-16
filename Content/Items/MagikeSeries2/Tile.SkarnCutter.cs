using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnCutter() : BasePlaceableItem(Item.sellPrice(0, 1), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<SkarnCutterTile>(), AssetDirectory.MagikeSeries2Item)
    {
    }
}
