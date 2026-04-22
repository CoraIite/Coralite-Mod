using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.IceDragonChapter1
{
    public class IceDragon1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.BabyIceDragon + "BabyIceDragon_Head_Boss";
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<BabyIceDragonPage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Wild;

        public override UIPage[] GetUIPages()
        {
            return [
                new BabyIceDragonPage1(),
                new IciclePage1(),
                ];
        }
    }
}
