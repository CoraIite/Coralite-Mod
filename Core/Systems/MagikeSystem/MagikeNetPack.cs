using System;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem
{
    public struct MagikeNetPack
    {
        public MagikeNetPack()
        {
            Position = Point16.NegativeOne;
        }

        public MagikeNetPack(Point16 position, MagikeNetPackType packType)
        {
            Position = position;
            PackType = packType;
        }

        public Point16 Position { get; set; }

        public MagikeNetPackType PackType { get; set; }

        /// <summary>
        /// 在发包时写入特定的信息
        /// </summary>
        public Action<ModPacket> WriteSpecialDatas { get; set; }
    }

    public enum MagikeNetPackType : byte
    {
        /// <summary>
        /// 用于同步魔能变动
        /// </summary>
        MagikeContainer_MagikeChange,
        /// <summary>
        /// 用于同步计时器的时间
        /// </summary>
        TimerTriger_Timer,
        /// <summary>
        /// 同步指定索引的物品
        /// </summary>
        ItemContainer_IndexedItem,
        /// <summary>
        /// 客户端 → 服务端：请求从物品容器取出一个物品（右键 Shift）。<br></br>
        /// 服务端权威生成掉落物并回传被清空的槽位，杜绝点击端本地 <c>Item.NewItem</c> 造成的多人不同步。
        /// </summary>
        ItemContainer_DropRequest,
        /// <summary>
        /// 客户端 → 服务端：切换充能器 UI 充能范围开关（ChargeItemsOnUp / ChargePlayerItemsOnUp）。
        /// </summary>
        Charger_ToggleOption,
        /// <summary>
        /// 客户端 → 服务端：断开线性发送器与指定索引接收者的连接。
        /// </summary>
        LinerSender_RemoveReceiver,
        /// <summary>
        /// 客户端 → 服务端：停止合成台工作并权威退还材料。
        /// </summary>
        CraftAltar_StopWork,
        /// <summary>
        /// 客户端 → 服务端：设置合成台 UI 模式（产出方式 / 自动选配方）。
        /// </summary>
        CraftAltar_SetMode,
    }
}
