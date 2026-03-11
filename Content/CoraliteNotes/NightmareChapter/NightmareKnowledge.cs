using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.NightmareChapter
{
    public class NightmareKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<NightmarePage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Ball;

        public override UIPageGroup GetUIPageGroup() => new GroupNightmare();
    }
}
