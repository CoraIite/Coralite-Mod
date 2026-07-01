using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    internal class MagikeInterstitial1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.Biomes + "MagicCrystalCaveIcon";
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial1Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Coral;

        public override UIPage[] GetUIPages()
        {
            return [
                new MagikeInterstitial1Page(),
                ];
        }
    }
}
