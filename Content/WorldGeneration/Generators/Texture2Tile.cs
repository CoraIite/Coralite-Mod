namespace Coralite.Content.WorldGeneration.Generators
{
    public class Texture2TileGenerator : TextureGenerator
    {
        public TileInfo[,] tileGen;

        public Texture2TileGenerator(int width, int height) : base(width, height)
        {
            tileGen = new TileInfo[width, height];
        }

        public void Generate(int x, int y, bool sync)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    TileInfo info = tileGen[x1, y1];

                    if (info.tileID != -1)
                        WorldGenHelper.Texture2TileGenerate(current_x, current_y, info.tileID, info.tileStyle, info.tileID > -1, info.liquidAmt == 0, false, sync);
                }
        }
    }
}
