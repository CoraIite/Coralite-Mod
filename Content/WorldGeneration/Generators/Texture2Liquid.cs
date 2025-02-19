using Terraria;

namespace Coralite.Content.WorldGeneration.Generators
{
    public class Texture2Liquid : TextureGenerator
    {
        public LiquidInfo[,] tileLiquidGen;

        public Texture2Liquid(int width, int height) : base(width, height)
        {
            tileLiquidGen = new LiquidInfo[width, height];
        }

        public void Generate(int x, int y, bool sync)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    LiquidInfo info = tileLiquidGen[x1, y1];

                    if (info.liquidID != -1)
                    {
                        WorldGen.PlaceLiquid(current_x, current_y, (byte)info.liquidID, 255);
                    }
                }
        }

    }
}
