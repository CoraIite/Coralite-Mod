using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.Components;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem
{
    /// <summary>
    /// Magike 系统 SyncVar 自定义类型注册（客户端与服务端均需执行）
    /// </summary>
    internal static class MagikeSyncRegistration
    {
        internal static void Register()
        {
            SyncVarManager.RegisterSyncType<Item[]>(WriteItemArray, ReadItemArray);

            //InnoVault 内置类型未含 ushort（仅有 short/Int16），仪器等级 ApparatusInformation.CurrentLevel 为 ushort，
            //在此补注册，使其可走 [SyncVar] 同步。
            SyncVarManager.RegisterSyncType<ushort>((writer, value) => writer.Write(value), reader => reader.ReadUInt16());

            SyncVarManager.RegisterSyncType<Point8>(WritePoint8, ReadPoint8);
            SyncVarManager.RegisterSyncType<List<Point8>>(WritePoint8List, ReadPoint8List);
        }

        private static void WritePoint8(BinaryWriter writer, Point8 point)
        {
            writer.Write(point.X);
            writer.Write(point.Y);
        }

        private static Point8 ReadPoint8(BinaryReader reader)
            => new(reader.ReadSByte(), reader.ReadSByte());

        private static void WritePoint8List(BinaryWriter writer, List<Point8> list)
        {
            list ??= [];
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
                WritePoint8(writer, list[i]);
        }

        private static List<Point8> ReadPoint8List(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            if (length < 0)
                length = 0;

            long remaining = reader.BaseStream.Length - reader.BaseStream.Position;
            if (length > remaining / 2)
                length = (int)(remaining / 2);

            var list = new List<Point8>(length);
            for (int i = 0; i < length; i++)
                list.Add(ReadPoint8(reader));

            return list;
        }

        private static void WriteItemArray(BinaryWriter writer, Item[] items)
        {
            items ??= [];
            writer.Write(items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                Item item = items[i];
                if (item == null || item.IsAir)
                    writer.Write(false);
                else
                {
                    writer.Write(true);
                    ItemIO.Send(item, writer, true);
                }
            }
        }

        private static Item[] ReadItemArray(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            if (length < 0)
                length = 0;

            //原则化的防爆上界（取代旧的 "999 挡刀" 补丁）：每个条目至少 1 字节（bool 标志），
            //因此数组长度不可能超过流中剩余字节数。配合 MagikeTP 的长度框，正常数据永远不会触发该夹取。
            long remaining = reader.BaseStream.Length - reader.BaseStream.Position;
            if (length > remaining)
                length = (int)remaining;

            var items = new Item[length];
            for (int i = 0; i < length; i++)
            {
                bool hasItem = reader.ReadBoolean();
                items[i] = hasItem ? ItemIO.Receive(reader, true) : new Item();
            }

            return items;
        }
    }
}
