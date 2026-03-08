using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.ThunderChapter1
{
    public class Thunder1Knowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ThunderveinDragonPage1>();
    }
}
