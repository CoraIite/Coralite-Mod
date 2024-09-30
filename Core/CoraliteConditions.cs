using Coralite.Content.WorldGeneration;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;

namespace Coralite.Core
{
    public class CoraliteConditions : ModSystem, ILocalizedModType
    {
        public static Condition LearnedMagikeBase { get; private set; }
        public static Condition CoralCat { get; private set; }
        public static Condition MagikeCraft { get; private set; }

        public string LocalizationCategory => "Conditions";

        public override void Load()
        {
            LearnedMagikeBase = new(this.GetLocalization(nameof(LearnedMagikeBase))
                , () => MagikeSystem.learnedMagikeBase);
            CoralCat = new(this.GetLocalization(nameof(CoralCat))
                , () => CoraliteWorld.coralCatWorld);
            MagikeCraft = new(this.GetLocalization(nameof(MagikeCraft))
                , () => false);
        }

        public override void Unload()
        {
        }
    }
}
