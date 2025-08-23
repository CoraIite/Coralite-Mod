using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    public class FairyFree
    {
        public static IFairyFreeRule Common(int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
            => new CommonItemSpawn(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator);

        public static IFairyFreeRule Common<T>(int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
            where T : ModItem
            => new CommonItemSpawn(ModContent.ItemType<T>(), chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator);

        public static IFairyFreeRule OneFromOption(int chanceDenominator, int chanceNumerator = 1, params int[] options)
            => new OneFromOptions(chanceDenominator, chanceNumerator, options);

        public static IFairyFreeRule Coin(long value, bool withRandomBonus = true)
            => new CoinsRule(value, withRandomBonus);

        public static void SpawnFairyAura(Vector2 pos, int fairyType, FairyRarity rarity, Player player)
        {
            FairyFreeAttempt attempt = new FairyFreeAttempt(pos, fairyType, rarity, player);

            //根据仙灵自身生成
            if (FairySystem.FairyAuraSpawners.TryGetValue(fairyType, out FairyFreeInfo info)
                && info.Rules != null)
                foreach (var rule in info.Rules)
                    rule.SpawnItem(ref attempt);

            if (!attempt.BlockRaritySpwan)
                return;

            //根据稀有度生成
            if (FairySystem.RarityAuraSpawners.TryGetValue((int)rarity, out FairyFreeInfo info2)
                && info2.Rules != null)
                foreach (var rule in info2.Rules)
                    rule.SpawnItem(ref attempt);
        }

        public static void CommonDrop(FairyFreeAttempt attempt, int itemType, int stack)
        {
            int index = Item.NewItem(new EntitySource_FairyFree(attempt.fairyType), attempt.pos
                , itemType, stack, noBroadcast: false, -1);

            Main.item[index].shimmered = true;

            if (VaultUtils.isClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index, 1f);
        }
    }
}
