using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigClearUpText { get; set; }

        public static void GenDigClear(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigClearUpText.Value;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                    if (Main.tile[i,j].HasTile)
                        WorldGen.TileFrame(i, j, true);

                progress.Value += 1f / Main.maxTilesX;
            }

            WorldGen.gen = false;
            WorldGen.noTileActions = false;
        }
    }
}
