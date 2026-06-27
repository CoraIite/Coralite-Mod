using Coralite.Core.Loaders;
using Coralite.Core.Network;
using Coralite.Helpers;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.WorldValueSystem
{
    public class WorldValueSystem : ModSystem
    {
        /*
         * 同步模型（P0 安全）：
         *
         * 情况1：玩家进入世界
         * 客户端 RequestForSync → 服务端 SendWorldValue(toWho) → 客户端 ReceiveWorldValue
         *
         * 情况2：服务端改变世界变量
         * SendWorldValue(-1) → 全体客户端 ReceiveWorldValue
         *
         * 情况3：客户端请求修改单个 flag
         * RequestFlagChange → 服务端校验 AcceptClientChangeRequest → 写入并 SendWorldValue(-1)
         *
         * 禁止：客户端整表 WriteBools 覆写服务端。
         */

        /// <summary>
        /// 在此记录所有的世界 bool 值
        /// </summary>
        public static bool[] WorldFlags { get; set; }

        internal static bool waitForSync;
        internal static int waitForSyncTime;
        internal static int syncRetryCount;
        internal static int syncRetryBackoff = InitialSyncBackoff;

        const int InitialSyncBackoff = 60 * 10;
        const int MaxSyncBackoff = 60 * 60;
        const int MaxSyncRetries = 5;

        public static bool Flag<T>() where T : WorldFlag
            => ModContent.GetInstance<T>().Value;

        public override void Load()
        {
            On_Main.UpdateTime_StartNight += StartNightWorldValueSync;
            On_Main.UpdateTime_StartDay += StartDayClientSyncRetryReset;
        }

        public override void Unload()
        {
            On_Main.UpdateTime_StartNight -= StartNightWorldValueSync;
            On_Main.UpdateTime_StartDay -= StartDayClientSyncRetryReset;
        }

        private static void StartDayClientSyncRetryReset(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
        {
            orig.Invoke(ref stopEvents);
            ResetClientSyncRetryState();
        }

        /// <summary>
        /// 重置客户端 WorldValue 同步重试计数（进世界、白天、成功收包时调用）。
        /// </summary>
        public static void ResetClientSyncRetryState()
        {
            if (!VaultUtils.isClient)
                return;

            syncRetryCount = 0;
            syncRetryBackoff = InitialSyncBackoff;
        }

        private void StartNightWorldValueSync(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
        {
            orig.Invoke(ref stopEvents);
            if (VaultUtils.isServer)
                SendWorldValue(-1);
        }

        public override void SetStaticDefaults()
        {
            WorldFlags = new bool[WorldValueLoader.FlagCount];
        }

        public override void PostUpdateTime()
        {
            if (!waitForSync)
                return;

            if (waitForSyncTime > 0)
            {
                waitForSyncTime--;
                if (waitForSyncTime < 1)
                    RequestForSync();
            }
        }

        /// <summary>
        /// 客户端向服务端请求权威世界变量整表（带最大重试与指数退避）。
        /// </summary>
        public static void RequestForSync()
        {
            if (!VaultUtils.isClient)
                return;

            if (syncRetryCount >= MaxSyncRetries)
            {
                Coralite.Instance.Logger.Warn("WorldValue 同步请求已达最大重试次数，停止重试");
                waitForSync = false;
                waitForSyncTime = 0;
                return;
            }

            waitForSync = true;
            waitForSyncTime = syncRetryBackoff;
            syncRetryCount++;

            if (syncRetryBackoff < MaxSyncBackoff)
                syncRetryBackoff = System.Math.Min(syncRetryBackoff * 2, MaxSyncBackoff);

            ModPacket p = Coralite.Instance.GetPacket();
            p.Write((byte)CoraliteNetWorkEnum.WorldValueRequest);
            p.Write(Main.myPlayer);
            p.Send();
        }

        /// <summary>
        /// 客户端请求修改单个世界 flag（不本地写入，等待服务端广播）。
        /// </summary>
        public static void RequestFlagChange(int flagType, bool value)
        {
            if (!VaultUtils.isClient)
                return;

            ModPacket p = Coralite.Instance.GetPacket();
            p.Write((byte)CoraliteNetWorkEnum.WorldFlagChangeRequest);
            p.Write(flagType);
            p.Write(value);
            p.Send();
        }

        public static void ServerHandleWorldValueRequest(BinaryReader reader, int fromWho)
        {
            if (!VaultUtils.isServer)
                return;

            reader.ReadInt32(); // 忽略包内 toWho，防伪造窥视/定向污染其它客户端
            if (fromWho < 0 || fromWho >= Main.maxPlayers || !Main.player[fromWho].active)
                return;

            SendWorldValue(fromWho);
        }

        public static void ServerHandleWorldFlagChangeRequest(BinaryReader reader, int fromWho)
        {
            if (!VaultUtils.isServer)
                return;

            int flagType = reader.ReadInt32();
            bool value = reader.ReadBoolean();

            if (WorldFlags == null || flagType < 0 || flagType >= WorldFlags.Length)
            {
                Coralite.Instance.Logger.Warn($"WorldFlagChangeRequest: 无效 flagType {flagType} from {fromWho}");
                return;
            }

            WorldFlag flag = WorldValueLoader.GetFlag(flagType);
            if (flag == null)
            {
                Coralite.Instance.Logger.Warn($"WorldFlagChangeRequest: 未注册 flag {flagType} from {fromWho}");
                return;
            }

            if (!flag.AcceptClientChangeRequest)
            {
                Coralite.Instance.Logger.Warn($"WorldFlagChangeRequest: 拒绝客户端修改进度 flag '{flag.Name}' from {fromWho}");
                return;
            }

            // 客户端请求仅允许单向解锁；禁止 value=false 回滚进度。
            if (!value)
            {
                Coralite.Instance.Logger.Warn($"WorldFlagChangeRequest: 拒绝回滚 flag '{flag.Name}' from {fromWho}");
                return;
            }

            if (WorldFlags[flagType])
                return;

            if (fromWho < 0 || fromWho >= Main.maxPlayers || !Main.player[fromWho].active)
            {
                Coralite.Instance.Logger.Warn($"WorldFlagChangeRequest: 无效请求者 {fromWho}");
                return;
            }

            Player player = Main.player[fromWho];
            if (!flag.TryAuthorizeClientUnlock(player))
            {
                Coralite.Instance.Logger.Warn($"WorldFlagChangeRequest: 授权失败 flag '{flag.Name}' from {fromWho}");
                return;
            }

            WorldFlags[flagType] = true;
            SendWorldValue(-1);
        }

        /// <summary>
        /// 服务端向客户端广播权威世界变量整表。仅服务端可调用。
        /// </summary>
        public static void SendWorldValue(int toWho)
        {
            if (!VaultUtils.isServer)
                return;

            ModPacket p = Coralite.Instance.GetPacket();
            p.Write((byte)CoraliteNetWorkEnum.WorldValue);
            p.WriteBools(WorldFlags);
            p.Send(toWho);
        }

        /// <summary>
        /// 客户端接收服务端下发的权威世界变量整表。
        /// </summary>
        public static void ReceiveWorldValue(BinaryReader reader)
        {
            if (VaultUtils.isServer)
                return;

            waitForSync = false;
            waitForSyncTime = 0;
            syncRetryCount = 0;
            syncRetryBackoff = InitialSyncBackoff;

            reader.ReadBools(WorldFlags);
        }

        public static void OnEnterWorld(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                ResetClientSyncRetryState();

            foreach (WorldFlag flag in WorldValueLoader.flags)
                flag.OnEnterWorld(player);
        }

        public override void PostWorldGen()
        {
            foreach (WorldFlag flag in WorldValueLoader.flags)
                if (flag.NeedResetPostWoldGen)
                    flag.Set(false);
        }

        public override void NetSend(BinaryWriter writer)
        {
        }

        public override void NetReceive(BinaryReader reader)
        {
        }
    }

    public class AAAAAWorldValueSaveLoad : ModSystem
    {
        public override void SaveWorldData(TagCompound tag)
        {
            for (int i = 0; i < WorldValueLoader.flags.Count; i++)
            {
                WorldFlag flag = WorldValueLoader.flags[i];
                bool value = WorldValueSystem.WorldFlags[i];

                if (value)
                    tag.Add(flag.Name, true);
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            for (int i = 0; i < WorldValueLoader.flags.Count; i++)
            {
                WorldFlag flag = WorldValueLoader.flags[i];
                WorldValueSystem.WorldFlags[i] = tag.ContainsKey(flag.Name);
            }
        }
    }
}
