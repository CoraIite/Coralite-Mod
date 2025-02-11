using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.ThunderChapter1
{
    public class GroupThunderChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge(KeyKnowledgeID.Thunder1).Unlock;

        public override void InitPages()
        {
            Pages = [
                new ThunderveinDragonPage1()
                ];
        }
    }
}
