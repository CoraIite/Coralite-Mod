using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    public class MagikeInterstitial3Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.Biomes + "CrystallineSkyIslandIcon";
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Coral;

        public override UIPage[] GetUIPages()
        {
            return [
                new MagikeInterstitial3Page(),
                new MagikeInterstitial3Page2(),
                new MagikeInterstitial3Page3(),
                new MagikeInterstitial3Slab1(),
                new MagikeInterstitial3Slab2(),
                new MagikeInterstitial3Slab3(),
                ];
        }
    }
}
