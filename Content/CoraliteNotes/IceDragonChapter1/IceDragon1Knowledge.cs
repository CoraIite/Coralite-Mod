using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.IceDragonChapter1
{
    public class IceDragon1Knowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<BabyIceDragonPage1>();
    }
}
