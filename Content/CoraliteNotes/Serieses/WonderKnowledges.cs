using Coralite.Content.CoraliteNotes.GlistentChapter;
using Coralite.Content.CoraliteNotes.NightmareChapter;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.Serieses
{
    internal class WonderKnowledges : KnowledgeSeries
    {
        public override float Priority => CoraliteSeriesPriorities.WonderKnowledges;

        public override void AddKnowledges()
        {
            AddKnowledge<GlistentKnowledge>();
            AddKnowledge<Slime1Knowledge>();
            AddKnowledge<NightmareKnowledge>();
        }
    }
}
