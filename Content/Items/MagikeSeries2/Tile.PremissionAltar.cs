using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class PremissionAltar() : BasePlaceableItem(Item.sellPrice(), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<PremissionAltarTile>(), AssetDirectory.MagikeSeries2Item)
    {
    }
}
