
namespace Coralite.Content.WorldGeneration.Generators
{
    public abstract class TextureGenerator
    {
        public int width, height;

        public TextureGenerator(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

}
