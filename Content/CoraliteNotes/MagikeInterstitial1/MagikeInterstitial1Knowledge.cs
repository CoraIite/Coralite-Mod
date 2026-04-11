using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    public class MagikeInterstitial1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.Biomes + "CrystallineSkyIslandIcon";
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial1Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Coral;

        public override UIPageGroup GetUIPageGroup() => new GroupMagikeInterstitial1();
    }
}
