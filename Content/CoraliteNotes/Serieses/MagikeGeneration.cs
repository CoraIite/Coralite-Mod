using Coralite.Content.CoraliteNotes.MagikeChapter1;
using Coralite.Content.CoraliteNotes.MagikeChapter2;
using Coralite.Content.CoraliteNotes.MagikeInterstitial1;
using Coralite.Content.CoraliteNotes.MagikeToolWeapon1;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.Serieses
{
    internal class MagikeGeneration : KnowledgeSeries
    {
        public override float Priority => CoraliteSeriesPriorities.MagikeGeneration;

        public override void AddKnowledges()
        {
            AddKnowledge<MagikeS1Knowledge>();
            AddKnowledge<MagikeToolWeapon1Knowledge>();
            AddKnowledge<MagikeInterstitial1Knowledge>();
            AddKnowledge<MagikeS2Knowledge>();
        }
    }
}
