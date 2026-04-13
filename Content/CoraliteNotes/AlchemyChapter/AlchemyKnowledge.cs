using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.AlchorthentSeries;
using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria.ID;

namespace Coralite.Content.CoraliteNotes.AlchemyChapter
{
    public class AlchemyKnowledge : CollectKnowledge
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + nameof(FaintEagle);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<AlchemyPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override int MainRewardItemType => ItemID.StoneBlock;//ModContent.ItemType<HephaesthRelic>();

        public override UIPageGroup GetUIPageGroup() => new GroupAlchemy();

        public override int GetCollectsCount() => (int)Alchs.Count;

        public enum Alchs
        {
            FaintEagle,
            RhombicMirror,
            Count,
        }
    }
}
