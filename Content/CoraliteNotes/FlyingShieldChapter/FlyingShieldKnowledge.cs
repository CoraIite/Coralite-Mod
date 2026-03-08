using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<FlyingShieldPage>();
    }
}
