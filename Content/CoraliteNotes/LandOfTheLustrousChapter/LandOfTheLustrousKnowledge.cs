using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    public class LandOfTheLustrousKnowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<LandOfTheLustrousPage>();
    }
}
