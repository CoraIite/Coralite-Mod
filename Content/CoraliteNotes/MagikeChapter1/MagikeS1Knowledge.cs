using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class MagikeS1Knowledge : KeyKnowledge
    {
        public override int Type => KeyKnowledgeID.MagikeS1;

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge1Page>();
    }
}
