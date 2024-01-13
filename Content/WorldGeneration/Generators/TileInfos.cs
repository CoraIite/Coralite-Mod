
namespace Coralite.Content.WorldGeneration.Generators
{
    public class TileInfo
    {
        public int tileID = -1;
        public int tileStyle;
        public int liquidAmt;

        public TileInfo(int tileID, int style)
        {
            this.tileID = tileID;
            tileStyle = style;
        }
    }

    public class WallInfo
    {
        public int wallID = -1;

        public WallInfo(int wallID)
        {
            this.wallID = wallID;
        }
    }

    public class TileObjectInfo
    {
        public int tileID = -1;
        public int tileStyle;

        public TileObjectInfo(int tileID, int style)
        {
            this.tileID = tileID;
            tileStyle = style;
        }
    }
}
