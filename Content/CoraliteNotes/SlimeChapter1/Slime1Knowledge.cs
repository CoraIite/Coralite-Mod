using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class Slime1Knowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SlimePage1>();
    }
}
