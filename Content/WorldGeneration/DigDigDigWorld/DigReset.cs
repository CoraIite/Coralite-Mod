using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText ReadyDig { get; set; }

        public static void DigReset(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = ReadyDig.Value;

            DigHellWidth = 0;

            if (WorldGen.genRand.NextBool(2))
                GenVars.crimsonLeft = false;
            else
                GenVars.crimsonLeft = true;

            WorldGen.gen = true;
            Liquid.ReInit();
            WorldGen.noTileActions = true;
            WorldGen.RandomizeWeather();
            Main.cloudAlpha = 0f;
            Main.maxRaining = 0f;
            Main.raining = false;

            Main.checkXMas();
            Main.checkHalloween();

            int num1086 = 86400;

            Main.slimeRainTime = -WorldGen.genRand.Next(num1086 * 2, num1086 * 3);
            Main.cloudBGActive = -WorldGen.genRand.Next(8640, 86400);

            WorldGen.crimson = WorldGen.genRand.NextBool(2);

            if (WorldGen.WorldGenParam_Evil == 0)
                WorldGen.crimson = false;

            if (WorldGen.WorldGenParam_Evil == 1)
                WorldGen.crimson = true;

            Main.worldID = WorldGen.genRand.Next(int.MaxValue);
            WorldGen.RandomizeTreeStyle();
            WorldGen.RandomizeCaveBackgrounds();
            WorldGen.RandomizeBackgrounds(WorldGen.genRand);
            WorldGen.RandomizeMoonState(WorldGen.genRand);

            GenVars.dungeonSide = (!WorldGen.genRand.NextBool(2)) ? 1 : (-1);
            DigDigDigWorldDungeonSide = GenVars.dungeonSide;

            int minValue3 = 15;
            int maxValue12 = 30;

            if (GenVars.dungeonSide == -1)
            {
                double num1089 = 1.0 - WorldGen.genRand.Next(minValue3, maxValue12) * 0.01;
                GenVars.jungleOriginX = (int)(Main.maxTilesX * num1089);
            }
            else
            {
                double num1090 = WorldGen.genRand.Next(minValue3, maxValue12) * 0.01;
                GenVars.jungleOriginX = (int)(Main.maxTilesX * num1090);
            }

            Main.spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileY = Main.maxTilesY / 2;

            Main.worldSurface = 12;
        }
    }
}
