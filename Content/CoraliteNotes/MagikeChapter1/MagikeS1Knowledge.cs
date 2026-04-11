using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class MagikeS1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + nameof(MagicCrystal);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge1Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Reel;

        public override UIPageGroup GetUIPageGroup() => new GroupMagikeChapter1();
    }
}
