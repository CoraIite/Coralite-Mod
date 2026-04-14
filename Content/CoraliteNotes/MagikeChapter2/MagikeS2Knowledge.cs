using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class MagikeS2Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + nameof(CrystallineMagike);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge2Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Reel;

        public override UIPage[] GetUIPages()
        {
            return [
                    new GetMagikeKnowledge2Page(),
                    new PartJumpPage2(),

                    //P1：神奇的小鸟物流
                    new ItemTransportation(),
                    new MabirdNestUI(),
                    new MabirdNestConnect(),
                    new MabirdLoupe(),
                ];
        }
    }
}
