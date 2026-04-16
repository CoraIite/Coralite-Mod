using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.Glistent;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.GlistentChapter
{
    public class GlistentKnowledge : Knowledge
    {
        public override string Texture => AssetDirectory.GlistentItems + nameof(GlistentBar);

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GlistentChapterPage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Metal;

        public override UIPage[] GetUIPages()
        {
            return [
                new GlistentChapterPage1()
                ];
        }
    }
}
