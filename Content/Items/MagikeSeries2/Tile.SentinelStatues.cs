using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SentinelStatues() : BasePlaceableItem(Item.sellPrice(), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<SentinelStatuesMain>(), AssetDirectory.MagikeSeries2Item)
    {
    }
}
