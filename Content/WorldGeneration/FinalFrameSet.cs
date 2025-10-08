using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public void FinalFrameSet(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].HasTile)
                    {
                        WorldGen.TileFrame(i, j, true, true);
                    }
                }
        }
    }
}
