using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    public class LandOfTheLustrousKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<LandOfTheLustrousPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override UIPageGroup GetUIPageGroup() => new GroupLandOfTheLustrous();
    }
}
