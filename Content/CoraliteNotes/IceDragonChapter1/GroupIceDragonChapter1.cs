using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.IceDragonChapter1
{
    public class GroupIceDragonChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<IceDragon1Knowledge>().Unlock;

        public override void InitPages()
        {
            Pages = [
                new BabyIceDragonPage1()
                ];
        }
    }
}
