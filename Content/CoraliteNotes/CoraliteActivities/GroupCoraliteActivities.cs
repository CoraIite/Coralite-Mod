using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.CoraliteActivities
{
    public class GroupCoraliteActivities : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<CoraliteActivitiesKnowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    new ActivityDescriptionPage(),
                    new StructrueActivityP1(),
                    new StructrueActivityP2(),
                ];
        }
    }
}
