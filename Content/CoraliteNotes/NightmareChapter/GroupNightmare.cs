using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.NightmareChapter
{
    public class GroupNightmare : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<NightmareKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new NightmarePage()
                ];
        }
    }
}
