using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    public class GroupMagikeInterstitial1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<MagikeInterstitial1Knowledge>().Unlock;

        public override void InitPages()
        {
            Pages = [
                new MagikeInterstitial1Page(),
                new MagikeInterstitial1Page2(),
                ];
        }
    }
}
