
namespace Coralite.Content.WorldGeneration.Generators
{
    public class Texture2WallGenerator : TextureGenerator
    {
        public WallInfo[,] wallGen;

        public Texture2WallGenerator(int width, int height) : base(width, height)
        {
            wallGen = new WallInfo[width, height];
        }

        public void Clear(int x, int y)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    WallInfo info = wallGen[x1, y1];

                    if (info.wallID != GenerateType.Ignore)
                        WorldGenHelper.Texture2WallGenerate(current_x, current_y, GenerateType.Clear);
                }
        }

        public void Generate(int x, int y, bool sync)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    WallInfo info = wallGen[x1, y1];

                    if (info.wallID != GenerateType.Ignore)
                        WorldGenHelper.Texture2WallGenerate(current_x, current_y, info.wallID);
                }
        }
    }
}
