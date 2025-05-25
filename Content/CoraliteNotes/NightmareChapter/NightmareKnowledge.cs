using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.NightmareChapter
{
    public class NightmareKnowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<NightmarePage>();
    }
}
