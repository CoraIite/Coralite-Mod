using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    public class LandOfTheLustrousKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<LandOfTheLustrousPage>();
    }
}
