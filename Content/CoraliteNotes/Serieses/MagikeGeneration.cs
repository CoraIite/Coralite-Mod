using Coralite.Content.CoraliteNotes.MagikeChapter1;
using Coralite.Content.CoraliteNotes.MagikeChapter2;
using Coralite.Content.CoraliteNotes.MagikeInterstitial1;
using Coralite.Content.CoraliteNotes.MagikeInterstitial2;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.CoraliteNotes.MagikeToolWeapon1;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.Serieses
{
    internal class MagikeGeneration : KnowledgeSeries
    {
        public override float Priority => CoraliteSeriesPriorities.MagikeGeneration;

        public override void AddKnowledges()
        {
            AddKnowledge<MagikeInterstitial1Knowledge>();
            AddKnowledge<MagikeS1Knowledge>();
            AddKnowledge<MagikeToolWeapon1Knowledge>();
            AddKnowledge<MagikeInterstitial2Knowledge>();
            AddKnowledge<MagikeInterstitial3Knowledge>();
            AddKnowledge<MagikeS2Knowledge>();
        }
    }
}
