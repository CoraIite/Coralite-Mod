using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    public class DashBowKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<DashBowPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override UIPageGroup GetUIPageGroup() => new GroupDashBowChapter();
    }
}
