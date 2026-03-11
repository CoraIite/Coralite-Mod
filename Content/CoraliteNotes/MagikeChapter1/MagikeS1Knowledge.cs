using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class MagikeS1Knowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge1Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Reel;

        public override UIPageGroup GetUIPageGroup() => new GroupMagikeChapter1();
    }
}
