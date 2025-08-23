using System;

namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    public class CommonItemSpawn : IFairyFreeRule
    {
        public int itemId;
        public int chanceDenominator;
        public int amountDroppedMinimum;
        public int amountDroppedMaximum;
        public int chanceNumerator;

        public CommonItemSpawn(int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
        {
            if (amountDroppedMinimum > amountDroppedMaximum)
            {
                throw new ArgumentOutOfRangeException(nameof(amountDroppedMinimum), $"{nameof(amountDroppedMinimum)} 必须小于等于 {nameof(amountDroppedMaximum)}.");
            }

            this.itemId = itemId;
            this.chanceDenominator = chanceDenominator;
            this.amountDroppedMinimum = amountDroppedMinimum;
            this.amountDroppedMaximum = amountDroppedMaximum;
            this.chanceNumerator = chanceNumerator;
        }

        public void SpawnItem(ref FairyFreeAttempt attempt)
        {
            if (attempt.player.RollLuck(chanceDenominator) < chanceNumerator)
                FairyFree.CommonDrop(attempt, itemId, attempt.rand.Next(amountDroppedMinimum, amountDroppedMaximum + 1));
        }
    }
}
