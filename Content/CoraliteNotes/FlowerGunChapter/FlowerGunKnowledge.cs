using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    public class FlowerGunKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunPage>();
    }
}
