using Coralite.Content.Dusts;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class StartSkyIsland() : BasePlaceableItem(Item.buyPrice(0, 2), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<StartSkyIslandTile>(), AssetDirectory.MagikeSeries2Item)
    {
    }

    public class StartSkyIslandTile() : BasePainting(3, 2
        , new Color(104, 172, 255), ModContent.DustType<SkarnDust>(), AssetDirectory.MagikeSeries2Tile)
    {
    }
}
