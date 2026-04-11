using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class MagikeS2Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + nameof(CrystallineMagike);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge2Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Reel;

        public override UIPageGroup GetUIPageGroup() => new GroupMagikeChapter2();
    }
}
