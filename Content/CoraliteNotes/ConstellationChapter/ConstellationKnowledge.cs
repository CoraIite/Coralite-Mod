using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.ConstellationChapter
{
    public class ConstellationKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ConstellationPage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Rune;

        public override UIPageGroup GetUIPageGroup() => new GroupConstellation();
    }
}
