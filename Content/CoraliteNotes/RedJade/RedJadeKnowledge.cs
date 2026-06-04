using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    public class RedJadeKnowledge : Knowledge
    {
        public override string Texture => AssetDirectory.Rediancie + "Rediancie_Head_Boss";

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<RedJadePage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Wild;

        public override UIPage[] GetUIPages()
        {
            return [
                    new RedJadePage(),
                    new RedJadeItemPage(),
                ];
        }
    }
}
