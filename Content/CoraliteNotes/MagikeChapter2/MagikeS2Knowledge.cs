using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class MagikeS2Knowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge2Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Reel;

        public override UIPageGroup GetUIPageGroup() => new GroupMagikeChapter2();
    }
}
