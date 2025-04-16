
namespace Coralite.Content.WorldGeneration.Generators
{
    public record TileInfo
    {
        public readonly Color color;
        public readonly int tileID = -1;
        public readonly int tileStyle;

        public TileInfo(Color color, int tileID, int style)
        {
            this.color = color;
            this.tileID = tileID;
            tileStyle = style;
        }
    }

    public record WallInfo
    {
        public readonly int wallID = -1;

        public WallInfo(int wallID)
        {
            this.wallID = wallID;
        }
    }

    public record LiquidInfo
    {
        public readonly int liquidID = -1;

        public LiquidInfo(int liquidID)
        {
            this.liquidID = liquidID;
        }
    }
}
