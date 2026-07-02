using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial2
{
    public class MagikeInterstitial2Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.CoraliteNote + "MagikeInterstitial2/MagikeI2Icon";
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial2Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Coral;

        public override UIPage[] GetUIPages()
        {
            return [
                new MagikeInterstitial2Page(),
                ];
        }
    }
}
