using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class Slime1Knowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SlimePage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Ball;

        public override UIPageGroup GetUIPageGroup() => new GroupSlimeChapter1();
    }
}
