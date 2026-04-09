using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class GroupFlyingShield : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKnowledge<FlyingShieldKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new FlyingShieldPage(),
                    new FlyingShieldCollect(),
                    new FlyingShieldAccessoryPage1(),
                    new FlyingShieldAccessoryPage2(),
                    new FlyingShieldAccessoryPage3(),
                    new FlyingShieldAccessoryPage4(),
                ];
        }
    }
}
