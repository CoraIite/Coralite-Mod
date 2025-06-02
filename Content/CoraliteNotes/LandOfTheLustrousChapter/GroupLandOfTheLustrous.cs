using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    public class GroupLandOfTheLustrous : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<LandOfTheLustrousKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new LandOfTheLustrousPage(),
                    new LandOfTheLustrousCollect(),
                ];
        }
    }
}
