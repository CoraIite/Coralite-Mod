using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    public class CoinsRule(long value, bool withRandomBonus = true) : IFairyFreeRule
    {
        public long value = value; // in copper coins
        public bool withRandomBonus = withRandomBonus;

        public void SpawnItem(ref FairyFreeAttempt attempt)
        {
            double scale = 1f;
            if (withRandomBonus)
            {
                scale += attempt.rand.Next(-20, 21) * .01f;
                if (attempt.rand.NextBool(5))
                    scale += attempt.rand.Next(5, 11) * .01f;
                if (attempt.rand.NextBool(10))
                    scale += attempt.rand.Next(10, 21) * .01f;
                if (attempt.rand.NextBool(15))
                    scale += attempt.rand.Next(15, 31) * .01f;
                if (attempt.rand.NextBool(20))
                    scale += attempt.rand.Next(20, 41) * .01f;
            }

            long money = (long)(value * scale);
            foreach ((int itemId, int count) in ToCoins(money))
                FairyFree.CommonDrop(attempt, itemId, count);
        }

        public static IEnumerable<(int itemId, int count)> ToCoins(long money)
        {
            int copper = (int)(money % 100);
            money /= 100;
            int silver = (int)(money % 100);
            money /= 100;
            int gold = (int)(money % 100);
            money /= 100;
            int plat = (int)money;

            if (copper > 0) 
                yield return (ItemID.CopperCoin, copper);
            if (silver > 0)
                yield return (ItemID.SilverCoin, silver);
            if (gold > 0)
                yield return (ItemID.GoldCoin, gold);
            if (plat > 0)
                yield return (ItemID.PlatinumCoin, plat);
        }
    }
}
