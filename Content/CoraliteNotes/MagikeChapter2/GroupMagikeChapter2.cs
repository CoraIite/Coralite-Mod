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
                    //new PartJumpPage(),

                    //P1：物流系统
                ];
        }
    }
}
