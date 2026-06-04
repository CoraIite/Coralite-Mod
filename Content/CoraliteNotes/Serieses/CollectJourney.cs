using Coralite.Content.CoraliteNotes.AlchemyChapter;
using Coralite.Content.CoraliteNotes.DashBowChapter;
using Coralite.Content.CoraliteNotes.FlowerGunChapter;
using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter;
using Coralite.Content.CoraliteNotes.SwordChapter;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.Serieses
{
    internal class CollectJourney : KnowledgeSeries
    {
        public override float Priority => CoraliteSeriesPriorities.CollectJourney;

        public override void AddKnowledges()
        {
            AddKnowledge<SwordKnowledge>();
            AddKnowledge<FlyingShieldKnowledge>();
            AddKnowledge<FlowerGunKnowledge>();
            AddKnowledge<DashBowKnowledge>();
            AddKnowledge<LandOfTheLustrousKnowledge>();
            AddKnowledge<AlchemyKnowledge>();
        }
    }
}
