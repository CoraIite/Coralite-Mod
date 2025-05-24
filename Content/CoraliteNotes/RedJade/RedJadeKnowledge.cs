using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    public class RedJadeKnowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<RedJadePage>();
    }
}
