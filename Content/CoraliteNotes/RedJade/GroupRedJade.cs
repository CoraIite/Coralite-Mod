using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    internal class GroupRedJade : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge(KeyKnowledgeID.RedJade).Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new RedJadePage()
                ];
        }
    }
}
