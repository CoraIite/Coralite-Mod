using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.NightmareChapter
{
    public class NightmareKnowledge : Knowledge
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "NightmarePlantera_Head_Boss";
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<NightmarePage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Ball;

        public override UIPage[] GetUIPages()
        {
            return [
                    new NightmarePage()
                ];
        }
    }
}
