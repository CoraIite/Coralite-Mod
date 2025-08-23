using Terraria;
using Terraria.Utilities;

namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    public struct FairyFreeAttempt(Vector2 pos, int fairyType, FairyRarity rarity, Player player)
    {
        public readonly Player player = player;

        public readonly int fairyType = fairyType;
        public readonly FairyRarity rarity = rarity;

        /// <summary>
        /// 物品生辰位置
        /// </summary>
        public readonly Vector2 pos = pos;

        public readonly UnifiedRandom rand => Main.rand;


        /// <summary>
        /// 设置为false阻止根据稀有度生成物品
        /// </summary>
        public bool BlockRaritySpwan = true;

    }
}
