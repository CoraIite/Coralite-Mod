using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    public class DashBowKnowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<DashBowPage>();
    }
}
