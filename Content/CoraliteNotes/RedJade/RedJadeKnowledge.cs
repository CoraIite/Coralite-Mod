using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    public class RedJadeKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<RedJadePage>();
    }
}
