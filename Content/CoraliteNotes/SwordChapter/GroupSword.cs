using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.SwordChapter
{
    public class GroupSwordChapter : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<SwordKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new SwordPage(),
                    new SwordCollect(),
                ];
        }
    }
}
