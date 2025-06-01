using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    public class GroupFlowerGun : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<FlowerGunKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new FlowerGunPage(),
                    new FlowerGunCollect(),
                ];
        }
    }
}
