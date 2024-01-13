using Terraria;

namespace Coralite.Content.WorldGeneration.Generators
{
    public class Texture2Object : TextureGenerator
    {
        public TileObjectInfo[,] tileObjectGen;

        public Texture2Object(int width, int height) : base(width, height)
        {
            tileObjectGen = new TileObjectInfo[width, height];
        }

        public void Generate(int x, int y, bool sync)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    TileObjectInfo info = tileObjectGen[x1, y1];

                    if (info.tileID != -1)
                    {
                        WorldGen.PlaceObject(current_x, current_y, info.tileID, true, info.tileStyle);
                    }
                }
        }

    }
}
