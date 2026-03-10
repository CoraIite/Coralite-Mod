using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.IceDragonChapter1
{
    public class IceDragon1Knowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<BabyIceDragonPage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Wild;

        public override UIPageGroup GetUIPageGroup() => new GroupIceDragonChapter1();
    }
}
