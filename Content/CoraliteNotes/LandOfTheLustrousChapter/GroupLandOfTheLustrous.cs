using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    public class GroupLandOfTheLustrous : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKnowledge<LandOfTheLustrousKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new LandOfTheLustrousPage(),
                    new LandOfTheLustrousCollect(),
                    new LandOfTheLustrousPage1(),
                    new LandOfTheLustrousPage2(),
                ];
        }
    }
}
