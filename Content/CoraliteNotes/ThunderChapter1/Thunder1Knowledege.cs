using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.ThunderChapter1
{
    public class Thunder1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Head_Boss";

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ThunderveinDragonPage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Wild;

        public override UIPageGroup GetUIPageGroup() => new GroupThunderChapter1();
    }
}
