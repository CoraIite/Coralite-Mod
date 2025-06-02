using Coralite.Content.Items.HyacinthSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;

namespace Coralite.Content.CustomHooks
{
    public class PureRoom:HookGroup
    {
        public override void Load()
        {
            On_WorldGen.GetTileTypeCountByCategory += On_WorldGen_GetTileTypeCountByCategory;
        }

        private int On_WorldGen_GetTileTypeCountByCategory(On_WorldGen.orig_GetTileTypeCountByCategory orig, int[] tileTypeCounts, TileScanGroup group)
        {
            switch (group)
            {
                case TileScanGroup.None:
                    return 0;
                case TileScanGroup.Corruption:
                    return tileTypeCounts[23] + tileTypeCounts[24] + tileTypeCounts[25] + tileTypeCounts[32]
                        + tileTypeCounts[112] + tileTypeCounts[163] + tileTypeCounts[400] + tileTypeCounts[398]
                        + -5 * tileTypeCounts[27] - 300 * tileTypeCounts[ModContent.TileType<HyacinthRelicTile>()];
                case TileScanGroup.Crimson:
                    return tileTypeCounts[199] + tileTypeCounts[203] + tileTypeCounts[200] + tileTypeCounts[401]
                        + tileTypeCounts[399] + tileTypeCounts[234] + tileTypeCounts[352] - 5 * tileTypeCounts[27] - 300 * tileTypeCounts[ModContent.TileType<HyacinthRelicTile>()];
                case TileScanGroup.Hallow:
                    return tileTypeCounts[109] + tileTypeCounts[110] + tileTypeCounts[113] + tileTypeCounts[117] + tileTypeCounts[116] + tileTypeCounts[164] + tileTypeCounts[403] + tileTypeCounts[402];
                case TileScanGroup.TotalGoodEvil:
                    {
                        int tileTypeCountByCategory = On_WorldGen_GetTileTypeCountByCategory(orig, tileTypeCounts, TileScanGroup.Hallow);
                        int tileTypeCountByCategory2 = On_WorldGen_GetTileTypeCountByCategory(orig, tileTypeCounts, TileScanGroup.Corruption);
                        int tileTypeCountByCategory3 = On_WorldGen_GetTileTypeCountByCategory(orig, tileTypeCounts, TileScanGroup.Crimson);
                        int num = 5 * tileTypeCounts[27] + 300 * tileTypeCounts[ModContent.TileType<HyacinthRelicTile>()];
                        int num2 = tileTypeCountByCategory2 + tileTypeCountByCategory3 + num;
                        return tileTypeCountByCategory - num2;
                    }
                default:
                    return 0;
            }
        }

        public override void Unload()
        {
            On_WorldGen.GetTileTypeCountByCategory -= On_WorldGen_GetTileTypeCountByCategory;
        }
    }
}
