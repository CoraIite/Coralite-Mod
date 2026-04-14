using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class Slime1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "SlimeEmperor_Head_Boss";

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SlimePage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Ball;

        public override UIPage[] GetUIPages()
        {
            return [
                new SlimePage1(),
                new SlimePage2(),
                ];
        }
    }
}
