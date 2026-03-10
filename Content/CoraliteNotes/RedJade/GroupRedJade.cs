using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    internal class GroupRedJade : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKnowledge<RedJadeKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new RedJadePage()
                ];
        }
    }
}
