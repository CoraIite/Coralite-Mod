using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.GlistentChapter
{
    public class GroupGlistentChapter : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKnowledge<GlistentKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages = [
                new GlistentChapterPage1()
                ];
        }
    }
}
