using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigHell { get; set; }

        private static int DigHellWidth;

        public static void GenDigHell(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigHell.Value;

            AshBar(progress);
            AshBlur(progress);
        }

        private static void AshBar(GenerationProgress progress)
        {
            DigHellWidth = Main.maxTilesX / 14 + WorldGen.genRand.Next(30);

            int hellside = GenVars.dungeonSide * -1;
            int x = hellside > 0 ? Main.maxTilesX : 0;

            for (int i = 0; i < DigHellWidth; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[x - i * hellside, j];
                    if (WorldGen.genRand.NextBool(40))
                        t.ResetToType(TileID.AshGrass);
                    else
                        t.ResetToType(TileID.Ash);
                }

            progress.Value = 0.25f;
        }

        private static void AshBlur(GenerationProgress progress)
        {
            int hellside = GenVars.dungeonSide * -1;
            int x = hellside > 0 ? Main.maxTilesX : 0;

            x -= DigHellWidth * hellside;

            var wr = BlurRandom(12);

            for (int j = 0; j < Main.maxTilesY; j++)
            {
                int width = WorldGen.genRand.Next(20);

                for (int i = 0; i < width; i++)//随机长度的刺刺
                {
                    Tile t = Main.tile[x - hellside * i, j];
                    t.ResetToType(TileID.Ash);
                }

                for (int i = 0; i < 6; i++)
                    Main.tile[x - hellside * (wr.Get() + width), j].ResetToType(TileID.Ash);
            }
        }
    }
}
