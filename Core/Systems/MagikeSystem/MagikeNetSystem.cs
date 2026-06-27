using Coralite.Core.Network;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static List<MagikeNetPack> MagikeNetPacks = new List<MagikeNetPack>();

        internal const string GUID = "CORA-MUPD";

        /// <summary>
        /// 服务端即时发送单条 Magike 子协议包（绕过 3 秒 batch 队列）。
        /// </summary>
        public static void SendImmediateMagikePack(MagikeNetPack pack, Action<ModPacket> writeSpecial = null)
        {
            if (!VaultUtils.isServer)
                return;

            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CoraliteNetWorkEnum.MagikeSystem);
            modPacket.Write(1);
            modPacket.Write(GUID);
            modPacket.WriteMagikePack(pack);
            writeSpecial?.Invoke(modPacket);
            modPacket.Send();
        }

        private int SendTime;

        public override void PostUpdateEverything()
        {
            SendTime++;
            if (SendTime < 60 * 3)//最多每3秒发一次包
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
                        int index = reader.ReadByte();
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
                    case MagikeNetPackType.ItemContainer_DropRequest:
                        //先读容器组件 ID 保持流对齐（普通/只读容器共用本路径）
                        int dropContainerId = reader.ReadInt32();
                        int dropSlotIndex = reader.ReadInt32();
                        //客户端取出请求只应由服务端权威执行（取出+清空槽位+回传同步）
                        if (VaultUtils.isServer
                            && entity.TryGetComponent(dropContainerId, out ItemContainer dropContainer))
                        {
                            if (dropSlotIndex >= 0)
                                dropContainer.ServerDropItem(dropSlotIndex);
                            else
                                dropContainer.ServerDropItem();
                        }
                        break;
                    case MagikeNetPackType.Charger_ToggleOption:
                        if (VaultUtils.isServer
                            && entity.TryGetComponent(MagikeComponentID.MagikeFactory, out MagikeFactory factory)
                            && factory is Charger charger)
                        {
                            charger.ApplyToggle((Charger.ChargerToggleType)reader.ReadByte());
                            entity.SendData();
                        }
                        break;
                    case MagikeNetPackType.LinerSender_RemoveReceiver:
                        int receiverIndex = reader.ReadInt32();
                        if (VaultUtils.isServer
                            && entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeSender sender)
                            && sender is MagikeLinerSender linerSender
                            && linerSender.Receivers.IndexInRange(receiverIndex))
                        {
                            linerSender.RemoveReceiver(linerSender.Receivers[receiverIndex]);
                        }
                        break;
                    case MagikeNetPackType.CraftAltar_StopWork:
                        if (VaultUtils.isServer
                            && entity.TryGetComponent(MagikeComponentID.MagikeFactory, out MagikeFactory craftFactory)
                            && craftFactory is CraftAltar craftAltar)
                            craftAltar.StopWork();
                        break;
                    case MagikeNetPackType.CraftAltar_SetMode:
                        if (VaultUtils.isServer
                            && entity.TryGetComponent(MagikeComponentID.MagikeFactory, out MagikeFactory modeFactory)
                            && modeFactory is CraftAltar modeAltar)
                        {
                            modeAltar.ApplyCraftMode((CraftAltar.CraftAltarModeType)reader.ReadByte(), reader.ReadByte());
                            entity.SendData();
                        }
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
