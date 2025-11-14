using Coralite.Core.Network;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static List<MagikeNetPack> MagikeNetPacks = new List<MagikeNetPack>();

        internal const string GUID = "CoraliteMagikeUpdate";

        private int SendTime;

        public override void PostUpdateEverything()
        {
            SendTime++;
            if (SendTime < 60)//最多每秒发一次包
                return;

            SendTime = 0;
            if (MagikeNetPacks.Count < 1 || !VaultUtils.isServer)//只有服务器应该执行该操作
                return;

            SendMagikePack();
            MagikeNetPacks.Clear();
        }

        public static void SendMagikePack()
        {
            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CoraliteNetWorkEnum.MagikeSystem);

            modPacket.Write(MagikeNetPacks.Count);

            foreach (var pack in MagikeNetPacks)
            {
                modPacket.Write(GUID);//分割

                modPacket.WriteMagikePack(pack);

                //根据不同的发包类型执行特定的操作
                pack.WriteSpecialDatas?.Invoke(modPacket);
            }

            modPacket.Send();
        }

        public static void ReceiveMagikePack(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                string id = reader.ReadString();

                if (id != GUID)
                    continue;

                reader.ReadMagikePack(out Point16 position, out MagikeNetPackType packType);

                if (!MagikeHelper.TryGetEntityWithTopLeft(position, out MagikeTP entity))
                    continue;

                switch (packType)
                {
                    case MagikeNetPackType.MagikeContainer_MagikeChange:
                        entity.GetMagikeContainer().ReceiveMagikeChange(reader);
                        break;
                    case MagikeNetPackType.TimerTriger_Timer:
                        int index = reader.ReadInt32();
                        int timer = reader.ReadInt32();

                        if (entity.ComponentsCache.IndexInRange(index) && entity.ComponentsCache[index] is ITimerTriggerComponent timerComponent)//只有存在这个才会继续运行
                        {
                            timerComponent.Timer = timer;
                        }
                        else
                        {
                            $"{position.X} {position.Y} 的位置找不到索引为 {index} 的计时器".DumpInConsole();
                            $"发生致命错误！".Dump();
                        }

                        break;
                    case MagikeNetPackType.ItemContainer_IndexedItem:
                        if (entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                            container.ReceiveIndexedItem(reader);
                        break;
                    default:
                        break;
                }
            }
        }

        #region 特定的接收操作



        #endregion
    }
}
