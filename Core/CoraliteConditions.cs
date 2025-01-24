using Coralite.Content.WorldGeneration;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core
{
    public class CoraliteConditions : ModSystem, ILocalizedModType
    {
        public static Condition LearnedMagikeBase { get; private set; }
        public static Condition LearnedMagikeAdvance { get; private set; }
        public static Condition CoralCat { get; private set; }
        public static Condition MagikeCraft { get; private set; }
        public static Condition InDigDigDig { get; private set; }
        public static Condition NotInDigDigDig { get; private set; }
        public static Condition UseMultiBlockStructure { get; private set; }

        public string LocalizationCategory => "Conditions";

        public static LocalizedText DownedGolemCondition { get; private set; }

        public override void Load()
        {
            LearnedMagikeBase = new(this.GetLocalization(nameof(LearnedMagikeBase))
                , () => MagikeSystem.learnedMagikeBase);
            LearnedMagikeAdvance = new(this.GetLocalization(nameof(LearnedMagikeAdvance))
                , () => MagikeSystem.learnedMagikeAdvanced);
            CoralCat = new(this.GetLocalization(nameof(CoralCat))
                , () => CoraliteWorld.CoralCatWorld);
            InDigDigDig = new(this.GetLocalization(nameof(InDigDigDig))
                , () => CoraliteWorld.DigDigDigWorld);
            NotInDigDigDig = new(this.GetLocalization(nameof(NotInDigDigDig))
                , () => !CoraliteWorld.DigDigDigWorld);

            MagikeCraft = new(this.GetLocalization(nameof(MagikeCraft))
                , () => false);
            UseMultiBlockStructure = new(this.GetLocalization(nameof(UseMultiBlockStructure))
                , () => false);

            DownedGolemCondition = this.GetLocalization(nameof(DownedGolemCondition));
        }

        public override void Unload()
        {
        }
    }
}
