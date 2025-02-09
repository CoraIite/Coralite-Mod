using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class Slime1Knowledge : KeyKnowledge
    {
        public override int Type => KeyKnowledgeID.Slime1;

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<BabyIceDragonPage1>();
    }
}
