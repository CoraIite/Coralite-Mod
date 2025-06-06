﻿using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using System;
using System.IO;
using System.Linq;
using Terraria.DataStructures;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 将自身添加到待发送列表中，需要传入发包类型
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="packType"></param>
        public static void AddToPackList(this MagikeComponent mp, MagikeNetPackType packType, Action<ModPacket> writeSpecial = null)
        {
            if (VaultUtils.isServer)
            {
                //只有特定类型的才会移除旧的然后再添加新的以减少发送的内容
                if (packType is not MagikeNetPackType.ItemContainer_IndexedItem)
                {
                    var oldPack = MagikeSystem.MagikeNetPacks.FirstOrDefault(p => p.Position == mp.Entity.Position && p.PackType == packType, new MagikeNetPack());
                    if (oldPack.Position != Point16.NegativeOne)
                        MagikeSystem.MagikeNetPacks.Remove(oldPack);
                }

                var pack = new MagikeNetPack(mp.Entity.Position, packType);

                if (writeSpecial != null)
                    pack.WriteSpecialDatas = writeSpecial;

                MagikeSystem.MagikeNetPacks.Add(pack);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modPacket"></param>
        /// <param name="point16"></param>
        /// <param name="packType"></param>
        public static void WriteMagikePack(this ModPacket modPacket, Point16 point16, MagikeNetPackType packType)
        {
            modPacket.Write(point16.X);
            modPacket.Write(point16.Y);
            modPacket.Write((byte)packType);
        }

        /// <summary>
        /// 将位置和类型写入
        /// </summary>
        /// <param name="modPacket"></param>
        /// <param name="pack"></param>
        public static void WriteMagikePack(this ModPacket modPacket, MagikeNetPack pack)
        {
            modPacket.Write(pack.Position.X);
            modPacket.Write(pack.Position.Y);
            modPacket.Write((byte)pack.PackType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="position"></param>
        /// <param name="packType"></param>
        public static void ReadMagikePack(this BinaryReader reader, out Point16 position, out MagikeNetPackType packType)
        {
            position = new Point16(reader.ReadInt16(), reader.ReadInt16());
            packType = (MagikeNetPackType)reader.ReadByte();
        }




        /// <summary>
        /// 将自身加入到列表中，同步时间，仅服务端会实际运行
        /// </summary>
        /// <param name="iTimer"></param>
        /// <param name="component"></param>
        public static void SendTimerComponentTime(this ITimerTriggerComponent iTimer, MagikeComponent component)
        {
            component.AddToPackList(MagikeNetPackType.TimerTriger_Timer, packet =>
            {
                packet.Write(component.IndexOfSelf());
                packet.Write(iTimer.Timer);
            });
        }
    }
}
