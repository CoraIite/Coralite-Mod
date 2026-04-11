using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    public class GroupDashBowChapter : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKnowledge<DashBowKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new DashBowPage(),
                    new DashBowCollect(),
                    new DashBowPage1(),
                ];
        }
    }
}
