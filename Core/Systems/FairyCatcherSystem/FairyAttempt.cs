using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public struct FairyAttempt(int wallType)
    {
        /// <summary>
        /// 墙壁类型
        /// </summary>
        public int wallType = wallType;
        /// <summary>
        /// 稀有度
        /// </summary>
        public Rarity rarity;
        /// <summary>
        /// 尝试的点位与中心点的距离
        /// </summary>
        public int WebRadius;

        /// <summary>
        /// 玩家
        /// </summary>
        public Player Player;
        /// <summary>
        /// 捕捉器物品，是玩家的手持物品
        /// </summary>
        public readonly Item Catcher => Player.HeldItem;






        public enum Rarity
        {
            /// <summary> 常见的 </summary>
            C,
            /// <summary> 不常见 </summary>
            U,
            /// <summary> 稀有 </summary>
            R,
            /// <summary> 双倍稀有 </summary>
            RR,
            /// <summary> 超级稀有 </summary>
            SR,
            /// <summary> 究极稀有 </summary>
            UR
        }
    }
}
