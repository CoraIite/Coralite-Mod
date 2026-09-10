using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

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
            if (!VaultUtils.isServer)
                return;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modPacket"></param>
        /// <param name="point16"></param>
        /// <param name="packType"></param>
        public static void WriteMagikePack(this BinaryWriter modPacket, Point16 point16, MagikeNetPackType packType)
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
        public static void WriteMagikePack(this BinaryWriter modPacket, MagikeNetPack pack)
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
        /// 写入bool数组
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="bools"></param>
        public static void WriteBools(this BinaryWriter pack, bool[] bools)
        {
            int count = bools.Length;
            int writeCount = 0;

            pack.Write(count);

            for (int i = 0; i < count / 8 + 1; i++)
            {
                BitsByte bt = new BitsByte();
                for (int j = 0; j < 8; j++)
                {
                    bt[j] = bools[writeCount];//写入对应的值

                    writeCount++;
                    if (writeCount > count - 1)
                        break;
                }

                pack.Write(bt);
            }
        }

        /// <summary>
        /// 读取bool数组
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="bools"></param>
        public static void ReadBools(this BinaryReader reader, bool[] bools)
        {
            int count = reader.ReadInt32();

            int k = 0;

            for (int i = 0; i < count / 8 + 1; i++)
            {
                BitsByte b = reader.ReadBitsByte();

                for (int j = 0; j < 8; j++)
                {
                    if (bools.Length - 1 < k)
                    {
                        "发生数组越界错误，请检查发包中的数组和接收的数组是不是一个东西！".Dump();
                        return;
                    }

                    bools[k] = b[j];
                    k++;
                    if (k > count - 1)
                        return;
                }
            }
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
