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

        public void GenMagikeShrines(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = MagicCrystalCaveText.Value;

            GenForestLensShrine();
        }

        private static void GenForestLensShrine()
        {
            int count = 1;
            if (Main.maxTilesX > 8000)
                count++;

            for (int i = 0; i < count; i++)
            {
                ForestLensData.DoLoad<ForestLensData>();
            }
        }
    }
}
