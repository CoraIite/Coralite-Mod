using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.ConstellationChapter
{
    public class GroupConstellation : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKnowledge<ConstellationKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages = [
                new ConstellationPage1()
                ];
        }
    }
}
