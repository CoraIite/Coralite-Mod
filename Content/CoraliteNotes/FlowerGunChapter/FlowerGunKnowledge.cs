using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    public class FlowerGunKnowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunPage>();
    }
}
