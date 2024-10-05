using Terraria.ID;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigDesert { get; set; }

        public static void GenDigDesert(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigDesert.Value;

            GenDesertBar(progress);
        }

        private static void GenDesertBar(GenerationProgress progress)
        {
            int desertside = GenVars.dungeonSide;

            int center = Main.maxTilesX / 2;
            int width = Main.maxTilesX / 10;
            int offset = Main.maxTilesX / 200;

            int desertCenter = center + desertside * (Main.maxTilesX / 5 + WorldGen.genRand.Next(-offset, offset));
            GenVars.desertHiveLeft = desertCenter - width / 2;
            GenVars.desertHiveRight = desertCenter + width / 2;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[GenVars.desertHiveLeft + i, j];
                    t.ResetToType(TileID.Sandstone);
                }

            progress.Value = 0.25f;
        }
    }
}
