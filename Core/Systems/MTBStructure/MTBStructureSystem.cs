using Terraria.Localization;

namespace Coralite.Core.Systems.MTBStructure
{
    public class MTBStructureSystem : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "Systems";

        public static LocalizedText FailText { get; private set; }

        public override void Load()
        {
            FailText = this.GetLocalization(nameof(FailText));
        }

        public override void Unload()
        {
            FailText = null;
        }
    }
}
