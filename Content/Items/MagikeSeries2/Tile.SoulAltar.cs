using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SoulOfLightAltar() : BasePlaceableItem(Item.sellPrice(), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<SoulOfLightAltarTile>(), AssetDirectory.MagikeSeries2Item)
    {
    }

    public class SoulOfNightAltar() : BasePlaceableItem(Item.sellPrice(), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<SoulOfNightAltarTile>(), AssetDirectory.MagikeSeries2Item)
    {
    }
}
