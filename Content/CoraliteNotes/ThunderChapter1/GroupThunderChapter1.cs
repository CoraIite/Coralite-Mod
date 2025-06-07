using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.ThunderChapter1
{
    public class GroupThunderChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<Thunder1Knowledge>().Unlock;

        public override void InitPages()
        {
            Pages = [
                new ThunderveinDragonPage1()
                ];
        }
    }
}
