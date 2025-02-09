using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.IceDragonChapter1
{
    internal class GroupIceDragonChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge(KeyKnowledgeID.IceDragon1).Unlock;

        public override void InitPages()
        {
            Pages = [
                new BabyIceDragonPage1()
                ];
        }
    }
}
