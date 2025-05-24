using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class MagikeS2Knowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge2Page>();
    }
}
