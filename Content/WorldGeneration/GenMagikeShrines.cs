using Coralite.Content.WorldGeneration.MagikeShrineDatas;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText MagikeShrinesText { get; set; }

        public static void GenMagikeShrines(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = MagicCrystalCaveText.Value;

            GenForestLensShrine();
            GenApparatusShrine();

            DesertLensData1.DoLoad<DesertLensData1>();
            HelltLensData1.DoLoad<HelltLensData1>();
            OceanLensData1.DoLoad<OceanLensData1>();
            GlowMushroomLensData1.DoLoad<GlowMushroomLensData1>();
        }

        private static void GenForestLensShrine()
        {
            int count = 1;
            if (Main.maxTilesX > 8000)
                count++;

            for (int i = 0; i < count; i++)
            {
                switch (Main.rand.Next(2))
                {
                    default:
                        break;
                    case 0:
                        ForestLensData1.DoLoad<ForestLensData1>();
                        break;
                    case 1:
                        ForestLensData1.DoLoad<ForestLensData2>();
                        break;
                }
            }
        }

        private static void GenApparatusShrine()
        {
            int count = 2;
            if (Main.maxTilesX > 6000)
                count += 2;
            if (Main.maxTilesX > 8000)
                count += 3;

            for (int i = 0; i < count; i++)
            {
                switch (Main.rand.Next(1))
                {
                    default:
                        break;
                    case 0:
                        ApparatusShrine1.DoLoad<ApparatusShrine1>();
                        break;
                    //case 1:
                    //    ForestLensData1.DoLoad<ForestLensData2>();
                    //    break;
                }
            }
        }
    }
}
