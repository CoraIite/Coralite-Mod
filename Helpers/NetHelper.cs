using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem;
using System;
using System.IO;
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
        public static void AddToPackList(this MagikeComponent mp,MagikeNetPackType packType,Action<ModPacket> writeSpecial=null)
        {
            if (VaultUtils.isServer)
            {
                var pack = new MagikeNetPack(mp.Entity.Position, packType);

                if (writeSpecial!=null)
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
    }
}
