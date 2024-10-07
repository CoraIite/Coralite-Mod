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
            PlaceLava(progress);
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

            progress.Value = 0.33f;
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

            progress.Value = 0.66f;
        }

        private static void PlaceLava(GenerationProgress progress)
        {
            int hellside = GenVars.dungeonSide * -1;
            int x = hellside > 0 ? Main.maxTilesX - 10 : 10;

            for (int i = 0; i < DigHellWidth-10; i++)
                for (int j = 20; j < Main.maxTilesY - 20; j++)
                    if (WorldGen.genRand.NextBool(130))
                    {
                        int x1 = x - hellside * i;
                        Tile top = Main.tile[x1, j - 1];
                        Tile bottom = Main.tile[x1, j + 1];
                        Tile left = Main.tile[x1 + 1, j];
                        Tile right = Main.tile[x1 - 1, j];

                        if (!top.HasTile || !bottom.HasTile || !left.HasTile || !right.HasTile)
                            continue;

                        if (Main.tile[x1, j].TileType != TileID.Ash)
                            continue;

                        Main.tile[x1, j].Clear(Terraria.DataStructures.TileDataType.Tile);
                        WorldGen.PlaceLiquid(x1, j, (byte)LiquidID.Lava, 255);
                    }

            progress.Value = 0.99f;
        }
    }
}
