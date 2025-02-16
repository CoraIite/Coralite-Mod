using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class GroupSlimeChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<Slime1Knowledge>().Unlock;

        public override void InitPages()
        {
            Pages = [
                new SlimePage1(),
                new SlimePage2(),
                ];
        }
    }
}
