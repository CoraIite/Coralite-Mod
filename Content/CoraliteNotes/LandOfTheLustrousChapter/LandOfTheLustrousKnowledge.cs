using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    public class LandOfTheLustrousKnowledge : CollectKnowledge
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + nameof(Pyrope);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<LandOfTheLustrousPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override int MainRewardItemType => ModContent.ItemType<LandOfTheLustrousRelic>();

        public override int GetCollectsCount() => (int)GemWeapons.Count;

        public enum GemWeapons
        {
            PyropeCrown,
            AmethystNecklace,
            AquamarineBracelet,
            PinkDiamondRose,
            ZumurudRing,
            PearlBrooch,
            RubyScepter,
            PeridotTalisman,
            SapphireHairpin,
            TourmalineMonoclastic,
            TopazMirror,
            ZirconGrail,

            Phosphophyllite,

            LandOfTheLustrous,
            Count,
        }

        public override UIPage[] GetUIPages()
        {
            return [
                    new LandOfTheLustrousPage(),
                    new LandOfTheLustrousCollect(),
                    new LandOfTheLustrousPage1(),
                    new LandOfTheLustrousPage2(),
                ];
        }
    }
}
