using System;

namespace Coralite.Content.WorldGeneration.Generators
{
    public class Texture2TileGenerator(int width, int height) : TextureGenerator(width, height)
    {
        public TileInfo[,] tileGen = new TileInfo[width, height];

        public void Clear(int x, int y)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    TileInfo info = tileGen[x1, y1];

                    if (info.tileID != GenerateType.Ignore)
                        WorldGenHelper.Texture2TileGenerate(current_x, current_y, GenerateType.Clear, 0, false);
                }
        }

        public void Generate(int x, int y)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    TileInfo info = tileGen[x1, y1];

                    if (info.tileID > -1)
                        WorldGenHelper.Texture2TileGenerate(current_x, current_y, info.tileID, info.tileStyle, info.tileID > -1);
                }
        }

        public void ObjectPlace(int x, int y, Action<Color, int, int> objectPlace)
        {
            for (int y1 = height - 1; y1 > -1; y1--)//倒着遍历
                for (int x1 = width - 1; x1 > -1; x1--)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    TileInfo info = tileGen[x1, y1];

                    objectPlace(info.color, current_x, current_y);
                }
        }
    }
}
