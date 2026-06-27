using Coralite.Core.Network;
using Coralite.Core.Network.DataModules;
using Coralite.Core.Systems.WorldValueSystem;
using InnoVault.DataModules;
using System.IO;
using Terraria;

namespace Coralite.Content.ModPlayers
{
    /// <summary>
    /// CoralitePlayer 多人同步模板：InnoVault <see cref="DataModuleStore"/>（存档 + <see cref="DataModuleStore.Clone"/>）
    /// 叠加 tML 三件套（<see cref="CopyClientState"/> / <see cref="SendClientChanges"/> / <see cref="SyncPlayer"/>）。
    ///
    /// 关键约束：<see cref="CopyClientState"/>、<see cref="SendClientChanges"/>、
    /// <see cref="WritePlayerSyncFields"/>、<see cref="ReadPlayerSyncFields"/> 必须维护<b>完全相同</b>的字段集合，
    /// 否则会刷包（多发）或误判（漏发）。新增可同步字段时四处同步修改。
    /// </summary>
    public partial class CoralitePlayer
    {
        internal const short MaxNightmareEnergyMax = 20;
        internal const int MaxNightmareCount = 100;

        /// <summary>可复用模块化存档/同步容器（武器域、知识域等 worker 可注册 <see cref="DataModule"/>）。</summary>
        public DataModuleStore Modules { get; private set; } = new();

        /// <summary>噩梦能量模块（武器 worker 后续可迁移字段至此）。</summary>
        public NightmareEnergyModule NightmareEnergyModule => Modules.Get<NightmareEnergyModule>();

        public override void Initialize()
        {
            Modules = new DataModuleStore();
        }

        /// <summary>
        /// 写出本玩家所有可同步字段（不含 enum 头与 whoAmI）。与 <see cref="ReadPlayerSyncFields"/> 严格对称。
        /// </summary>
        internal void WritePlayerSyncFields(BinaryWriter writer)
        {
            writer.Write(nightmareEnergy);
            writer.Write(nightmareEnergyMax);
            writer.Write(nightmareCount);
        }

        /// <summary>
        /// 读入本玩家所有可同步字段。与 <see cref="WritePlayerSyncFields"/> 严格对称。
        /// </summary>
        internal void ReadPlayerSyncFields(BinaryReader reader)
        {
            short energy = reader.ReadInt16();
            short energyMax = reader.ReadInt16();
            int count = reader.ReadInt32();

            if (!TryValidateSyncFields(energy, energyMax, count, out _))
                return;

            nightmareEnergy = energy;
            nightmareEnergyMax = energyMax;
            nightmareCount = count;
        }

        internal static bool TryValidateSyncFields(short energy, short energyMax, int count, out string rejectReason)
        {
            if (energyMax < 0 || energyMax > MaxNightmareEnergyMax)
            {
                rejectReason = $"energyMax={energyMax}";
                return false;
            }

            if (energy < 0 || energy > energyMax)
            {
                rejectReason = $"energy={energy} max={energyMax}";
                return false;
            }

            if (count < 0 || count > MaxNightmareCount)
            {
                rejectReason = $"count={count}";
                return false;
            }

            rejectReason = null;
            return true;
        }

        /// <summary>
        /// 发送本玩家可同步字段。客户端发往服务端，服务端中继其它客户端。
        /// </summary>
        /// <param name="toWho">目标客户端，-1 为广播。</param>
        /// <param name="fromWho">忽略的客户端（一般为原发送者）。</param>
        public void SendPlayerSync(int toWho = -1, int fromWho = -1)
        {
            if (VaultUtils.isSinglePlayer)
                return;

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CoraliteNetWorkEnum.SyncCoralitePlayer);
            packet.Write((byte)Player.whoAmI);
            WritePlayerSyncFields(packet);
            packet.Send(toWho, fromWho);
        }

        /// <summary>
        /// 客户端：复制可同步字段到快照副本，供下一帧 <see cref="SendClientChanges"/> 比对。
        /// </summary>
        public override void CopyClientState(ModPlayer targetCopy)
        {
            CoralitePlayer copy = (CoralitePlayer)targetCopy;
            copy.nightmareEnergy = nightmareEnergy;
            copy.nightmareEnergyMax = nightmareEnergyMax;
            copy.nightmareCount = nightmareCount;
            copy.Modules = Modules.Clone();
        }

        /// <summary>
        /// 客户端：与上一帧快照比对，发生变化时发包同步（字段集合须与 <see cref="CopyClientState"/> 一致）。
        /// </summary>
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            CoralitePlayer snapshot = (CoralitePlayer)clientPlayer;

            bool changed = snapshot.nightmareEnergy != nightmareEnergy
                || snapshot.nightmareEnergyMax != nightmareEnergyMax
                || snapshot.nightmareCount != nightmareCount;

            if (changed)
                SendPlayerSync();
        }

        /// <summary>
        /// 玩家加入世界时的完整同步：广播本玩家字段，并请求权威世界变量整表。
        /// </summary>
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            SendPlayerSync(toWho, fromWho);

            if (newPlayer)
                WorldValueSystem.RequestForSync();
        }
    }
}
