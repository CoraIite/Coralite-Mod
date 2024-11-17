using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class GroupMagikeChapter1:UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge(KeyKnowledgeID.MagikeS1).Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new GetMagikeKnowledge1Page(),
                ];
        }
    }

}
