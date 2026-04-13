using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.AlchemyChapter
{
    public class GroupAlchemy : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKnowledge<AlchemyKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new AlchemyPage(),
                ];
        }
    }
}
