using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.CoraliteNotes.RedJade;
using Coralite.Content.CoraliteNotes.ThunderChapter1;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.Serieses
{
    public class TerrariaJourney : KnowledgeSeries
    {
        public override float Priority => CoraliteSeriesPriorities.TerrariaJourney;

        public override void AddKnowledges()
        {
            AddKnowledge<RedJadeKnowledge>();
            AddKnowledge<IceDragon1Knowledge>();
            AddKnowledge<Thunder1Knowledge>();
        }
    }
}
