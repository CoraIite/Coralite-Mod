using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    public class MagikeInterstitial1Knowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial1Page>();
    }
}
