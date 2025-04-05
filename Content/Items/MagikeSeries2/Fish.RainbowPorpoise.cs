using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class RainbowPorpoise() : BaseQuestFish(AssetDirectory.MagikeSeries2Item)
    {
        public override bool QuestAvailable => CoraliteWorld.HasPermission&&Main.hardMode;

        public override LocalizedText Description => DescriptionText;

        public override LocalizedText CatchLocation => CatchLocationText;

        public static LocalizedText DescriptionText;
        public static LocalizedText CatchLocationText;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            DescriptionText = this.GetLocalization(nameof(Description));
            CatchLocationText = this.GetLocalization(nameof(CatchLocation));
        }
    }
}
