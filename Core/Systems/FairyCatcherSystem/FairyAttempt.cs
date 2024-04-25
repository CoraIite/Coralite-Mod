using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public struct FairyAttempt
    {
        /// <summary>
        /// 墙壁类型
        /// </summary>
        public int wallType;
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

        /// <summary>
        /// 玩家目前所使用的鱼饵物品
        /// </summary>
        public Item baitItem;

        /// <summary>
        /// 随机生成仙灵点位的X坐标，是物块坐标
        /// </summary>
        public int X;
        /// <summary>
        /// 随机生成仙灵点位的Y坐标，是物块坐标
        /// </summary>
        public int Y;

        public enum Rarity
        {
            /// <summary> 常见的，淡黄色 </summary>
            C = 0,
            /// <summary> 不常见，青绿色 </summary>
            U = 10,
            /// <summary> 稀有，天蓝色 </summary>
            R = 20,
            /// <summary> 双倍稀有，蓝色 </summary>
            RR = 30,
            /// <summary> 史诗，超级稀有，红色 </summary>
            SR = 40,
            /// <summary> 神话，究极稀有，橙色 </summary>
            UR = 50,

            //-----------------------------------------------------------------------
            //                  以下为只有使用特殊鱼饵才能够出现的稀有度
            //-----------------------------------------------------------------------

            /// <summary> 三倍稀有，蓝紫色 </summary>
            RRR = 35,
            /// <summary> 传说，Hyper稀有，粉色 </summary>
            HR = 60,
            /// <summary> 异类稀有，粉色 </summary>
            AR = 25,
            /// <summary> 秘宝，金色 </summary>
            MR = 100,
        }
    }
}
