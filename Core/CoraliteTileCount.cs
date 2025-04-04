using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.Tiles.MagikeSeries2;
using System;

namespace Coralite.Core
{
    public class CoraliteTileCount:ModSystem
    {
        public int CrystalCaveTileCount;
        public int CrystallineSkyIslandTileCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            //魔力水晶洞的环境物块
            CrystalCaveTileCount = tileCounts[ModContent.TileType<BasaltTile>()];
            CrystalCaveTileCount += tileCounts[ModContent.TileType<HardBasaltTile>()];
            CrystalCaveTileCount += tileCounts[ModContent.TileType<CrystalBasaltTile>()];
            CrystalCaveTileCount += tileCounts[ModContent.TileType<MagicCrystalBlockTile>()];

            //蕴魔空岛的环境物块
            CrystallineSkyIslandTileCount = tileCounts[ModContent.TileType<SkarnTile>()];
            CrystallineSkyIslandTileCount += tileCounts[ModContent.TileType<SmoothSkarnTile>()];
            CrystallineSkyIslandTileCount += tileCounts[ModContent.TileType<SkarnBrickTile>()];
            CrystallineSkyIslandTileCount += tileCounts[ModContent.TileType<SkarnBrickTile>()];
            CrystallineSkyIslandTileCount += tileCounts[ModContent.TileType<CrystallineSkarnTile>()];
            CrystallineSkyIslandTileCount += tileCounts[ModContent.TileType<ChalcedonySkarn>()];
            CrystallineSkyIslandTileCount += tileCounts[ModContent.TileType<ChalcedonySmoothSkarn>()];
        }

        public bool InCrystalCave => CrystalCaveTileCount > 500;
        public bool InCrystallineSkyIsland => CrystallineSkyIslandTileCount > 400;
    }
}
