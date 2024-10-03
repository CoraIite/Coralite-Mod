using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheJungle { get; set; }

        public void GenDigJungle(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = StoneBack.Value;

        }
    }
}
