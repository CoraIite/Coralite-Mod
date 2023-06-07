using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ObjectData;

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

        public static bool HasSolidTile(this Tile tile)
        {
            return tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Main.tile[x, y]; 
            if (tile == null) 
                return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;
            return new Vector2(x, y);
        }
    }
}
