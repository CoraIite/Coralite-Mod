using Coralite.Content.CoraliteNotes.CoraliteActivities;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.Serieses
{
    internal class HideActivities : KnowledgeSeries
    {
        public override float Priority => CoraliteSeriesPriorities.HideActivities;

        public override bool HasDirectoryPage => false;

        public override void AddKnowledges()
        {
            AddKnowledge<CoraliteActivitiesKnowledge>();
        }
    }
}
