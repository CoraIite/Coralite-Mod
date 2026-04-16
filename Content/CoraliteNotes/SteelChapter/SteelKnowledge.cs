using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.Steel;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.SteelChapter
{
    public class SteelKnowledge : Knowledge
    {
        public override string Texture => AssetDirectory.SteelItems + nameof(SteelBar);

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Metal;

        public override UIPage[] GetUIPages()
        {
            return [
                new SteelPage1()
                ];
        }
    }
}
