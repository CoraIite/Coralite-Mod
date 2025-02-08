using Terraria;

namespace Coralite.Core
{
    public interface IDashable
    {
        /// <summary>
        /// 冲刺优先级
        /// </summary>
        float Priority { get; }

        /// <summary>
        /// 优先度最高：手持物品
        /// </summary>
        public const float HeldItemDash = 0;
        /// <summary>
        /// 有限度高的饰品
        /// </summary>
        public const float AccessoryDashHigh = 10;
        /// <summary>
        /// 有限度低的饰品
        /// </summary>
        public const float AccessoryDashLow = 100;

        bool Dash(Player Player, int DashDir);
    }
}
