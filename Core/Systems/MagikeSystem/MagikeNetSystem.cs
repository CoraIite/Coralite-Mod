using Coralite.Helpers;
using System.Collections.Generic;
using System.IO;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static List<MagikeNetPack> MagikeNetPacks = new List<MagikeNetPack>();

        internal const string GUID = "CoraliteMod_GUIDValue-YI58-FA38-GN58";

        public override void PostUpdateEverything()
        {
            if (MagikeNetPacks.Count < 1 || !VaultUtils.isServer)//只有服务器应该执行该操作
                return;

            SendMagikePack();
            MagikeNetPacks.Clear();
        }

        public void SendMagikePack()
        {
            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CoraliteNetWorkEnum.MagikeSystem);

            foreach (var pack in MagikeNetPacks)
            {
                modPacket.WriteMagikePack(pack);

                //根据不同的发包类型执行特定的操作
                pack.WriteSpecialDatas?.Invoke(modPacket);

                modPacket.Write(GUID);//分割
            }

            modPacket.Send();
        }

        public static void ReceiveMagikePack(BinaryReader reader)
        {

        }

        #region 特定的接收操作



        #endregion
    }
}
