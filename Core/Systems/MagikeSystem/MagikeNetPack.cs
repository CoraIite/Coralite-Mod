using System;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem
{
    public struct MagikeNetPack
    {
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
        Container_MagikeChange,
    }

}
