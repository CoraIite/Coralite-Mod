using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class GroupFlyingShield : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<FlyingShieldKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new FlyingShieldPage()
                ];
        }
    }
}
