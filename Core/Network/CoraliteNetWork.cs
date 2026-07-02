using Coralite.Content.Biskety;
using Coralite.Content.Items.Magike.Tools;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.NPCs.Town;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.WorldValueSystem;
using System.IO;
using Terraria;

namespace Coralite.Core.Network
{
    public class CoraliteNetWork
    {
        public static void NetWorkHander(BinaryReader reader, int whoAmI)
        {
            CoraliteNetWorkEnum coraliteNetWorkEnum = (CoraliteNetWorkEnum)reader.ReadByte();

            switch (coraliteNetWorkEnum)
            {
                case CoraliteNetWorkEnum.PlaceFilter:
                    FilterProj.Hander_PlaceFilter(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.FilterRemoveButton_LeftClick:
                    FilterRemoveButton.Hander_LeftClick_Data(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.ClusterWand:
                    InfinityClusterWandProj.Hander_ClusterWand(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.BrilliantConnectStaff_Sender:
                    BrilliantConnectStaffProj.Hander_Sender(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.BrilliantConnectStaff_Receivers:
                    BrilliantConnectStaffProj.Hander_Receivers(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.ItemContainer_SpecificIndex:
                    ItemContainer.ReceiveSpecificItem(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.ItemContainer:
                    ItemContainer.ReceiveItem(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.MagikeSystem:
                    MagikeSystem.ReceiveMagikePack(reader);
                    break;
                case CoraliteNetWorkEnum.KillBiskety:
                    if (VaultUtils.isServer)
                        BisketyHead.KillBiskety();
                    break;
                case CoraliteNetWorkEnum.SpawnBiskety:
                    if (VaultUtils.isServer && whoAmI >= 0 && whoAmI < Main.maxPlayers && Main.player[whoAmI].active)
                        BisketyHead.SpawnBiskety(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.WorldValueRequest:
                    WorldValueSystem.ServerHandleWorldValueRequest(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.WorldValue:
                    WorldValueSystem.ReceiveWorldValue(reader);
                    break;
                case CoraliteNetWorkEnum.WorldFlagChangeRequest:
                    WorldValueSystem.ServerHandleWorldFlagChangeRequest(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.TownNPCStatueTeleport:
                    HandleTownNPCStatueTeleport(reader);
                    break;
                case CoraliteNetWorkEnum.UnlockKnowledge:
                    KnowledgeSystem.HandleUnlockKnowledge(reader);
                    break;
                case CoraliteNetWorkEnum.RequestSpawnNPC:
                    NetAuthority.ServerHandleSpawnRequest(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.RequestKillNPC:
                    NetAuthority.ServerHandleKillRequest(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.SyncCoralitePlayer:
                    HandleSyncCoralitePlayer(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.UnlockSlabs:
                    DownedBossSystem.SlabText();
                    break;
                default:
                    Coralite.Instance.Logger.Warn($"未知 CoraliteNetWorkEnum {(byte)coraliteNetWorkEnum} from {whoAmI}");
                    return;
            }
        }

        /// <summary>
        /// 客户端播放城镇 NPC 雕像传送特效；服务端权威传送由原版处理。
        /// </summary>
        private static void HandleTownNPCStatueTeleport(BinaryReader reader)
        {
            if (VaultUtils.isServer)
                return;

            int npcWhoAmI = reader.ReadInt32();
            if (npcWhoAmI < 0 || npcWhoAmI >= Main.maxNPCs)
                return;

            NPC npc = Main.npc[npcWhoAmI];
            if (!npc.active)
                return;

            if (npc.ModNPC is ElfRanger elfRanger)
                elfRanger.StatueTeleport();
            else if (npc.ModNPC is CrystalRobot crystalRobot)
                crystalRobot.StatueTeleport();
        }

        /// <summary>
        /// 接收 <see cref="CoraliteNetWorkEnum.SyncCoralitePlayer"/>：写回对应玩家可同步字段；服务端再中继给其它客户端。
        /// </summary>
        private static void HandleSyncCoralitePlayer(BinaryReader reader, int sender)
        {
            int playerWhoAmI = reader.ReadByte();
            if (playerWhoAmI < 0 || playerWhoAmI >= Main.maxPlayers)
                return;

            if (sender != playerWhoAmI)
            {
                Coralite.Instance.Logger.Warn($"SyncCoralitePlayer: 拒绝伪造 sender={sender} target={playerWhoAmI}");
                return;
            }

            Player player = Main.player[playerWhoAmI];
            if (!player.active || !player.TryGetModPlayer(out Content.ModPlayers.CoralitePlayer coralitePlayer))
                return;

            short energy = reader.ReadInt16();
            short energyMax = reader.ReadInt16();
            int count = reader.ReadInt32();

            if (!Content.ModPlayers.CoralitePlayer.TryValidateSyncFields(energy, energyMax, count, out string rejectReason))
            {
                Coralite.Instance.Logger.Warn($"SyncCoralitePlayer: 拒绝非法字段 from {sender}: {rejectReason}");
                return;
            }

            coralitePlayer.nightmareEnergy = energy;
            coralitePlayer.nightmareEnergyMax = energyMax;
            coralitePlayer.nightmareCount = count;

            if (VaultUtils.isServer)
                coralitePlayer.SendPlayerSync(-1, sender);
        }
    }
}
