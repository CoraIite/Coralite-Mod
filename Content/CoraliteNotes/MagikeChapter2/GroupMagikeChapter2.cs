using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class GroupMagikeChapter2 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<MagikeS2Knowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
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
