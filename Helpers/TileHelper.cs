
using Terraria;

namespace Coralite.Helpers
{
    public static class TileHelper
    {
        public static bool topSlope(this Tile tile)
        {
            byte b = (byte)tile.Slope;
            if (b != 1)
                return b == 2;

            return true;
        }
    }
}
