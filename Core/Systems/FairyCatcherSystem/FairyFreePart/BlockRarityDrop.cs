namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    public class BlockRarityDrop : IFairyFreeRule
    {
        public void SpawnItem(ref FairyFreeAttempt attempt)
        {
            attempt.BlockRaritySpwan = true;
        }
    }
}
