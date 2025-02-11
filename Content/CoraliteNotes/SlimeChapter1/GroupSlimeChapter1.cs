using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class GroupSlimeChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge(KeyKnowledgeID.Slime1).Unlock;

        public override void InitPages()
        {
            Pages = [
                new SlimePage1(),
                new SlimePage2(),
                ];
        }
    }
}
