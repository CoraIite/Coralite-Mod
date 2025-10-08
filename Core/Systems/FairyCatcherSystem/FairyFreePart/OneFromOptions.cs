namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    public class OneFromOptions(int chanceDenominator, int chanceNumerator = 1, params int[] options) : IFairyFreeRule
    {
        public int[] dropIds = options;
        public int chanceDenominator = chanceDenominator;
        public int chanceNumerator = chanceNumerator;

        public void SpawnItem(ref FairyFreeAttempt attempt)
        {
            if (attempt.player.RollLuck(chanceDenominator) < chanceNumerator)
                FairyFree.CommonDrop(attempt, dropIds[attempt.rand.Next(dropIds.Length)], 1);
        }
    }
}
