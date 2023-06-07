
namespace Coralite.Content.WorldGeneration.Generators
{
    public class TileInfo
    {
        public int tileID = 0;
        public int tileStyle;
        public int liquidAmt;

        public TileInfo(int tileID, int style)
        {
            this.tileID = tileID;
            tileStyle = style;
        }

    }
}
