using Coralite.Content.Biskety;
using Coralite.Content.NPCs.Town;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Network
{
    /// <summary>
    /// 跨域网络权威门面：客户端发请求，服务端执行并同步；单人/服务端可直接调用。
    /// <para>禁止将 <see cref="CoraliteNetWorkEnum.RequestSpawnNPC"/> / <see cref="CoraliteNetWorkEnum.RequestKillNPC"/> 暴露给未校验的客户端逻辑；仅通过本类 API 发起。</para>
    /// </summary>
    public static class NetAuthority
    {
        const float MaxSpawnDistanceFromPlayer = 2000f;
        const int SpawnRequestCooldownTicks = 60;
        const int KillRequestCooldownTicks = 30;

        static readonly HashSet<int> ClientSpawnWhitelist = new();
        static readonly HashSet<int> ClientKillWhitelist = new();
        static bool whitelistInitialized;
        static int[] lastSpawnRequestTick;
        static int[] lastKillRequestTick;

        static void EnsureWhitelistInitialized()
        {
            if (whitelistInitialized)
                return;

            whitelistInitialized = true;
            ClientSpawnWhitelist.Add(ModContent.NPCType<CrystalRobot>());
            ClientKillWhitelist.Add(ModContent.NPCType<Biskety>());
            lastSpawnRequestTick = new int[Main.maxPlayers];
            lastKillRequestTick = new int[Main.maxPlayers];
        }

        /// <summary>
        /// 生成 NPC。单人或服务端直接 <see cref="NPC.NewNPC"/> 并同步；客户端发送 <see cref="CoraliteNetWorkEnum.RequestSpawnNPC"/>。
        /// </summary>
        /// <returns>新 NPC 的 whoAmI；客户端请求时返回 -1。</returns>
        public static int SpawnNPC(IEntitySource source, int x, int y, int type, int start = 0, float ai0 = 0f, float ai1 = 0f, int target = 255)
        {
            if (VaultUtils.isSinglePlayer || VaultUtils.isServer)
            {
                int index = NPC.NewNPC(source, x, y, type, start, ai0, ai1, target);
                if (VaultUtils.isServer && index >= 0 && index < Main.maxNPCs)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);

                return index;
            }

            ModPacket packet = Coralite.Instance.GetPacket();
            packet.Write((byte)CoraliteNetWorkEnum.RequestSpawnNPC);
            packet.Write(x);
            packet.Write(y);
            packet.Write(type);
            packet.Write((byte)start);
            packet.Write(ai0);
            packet.Write(ai1);
            packet.Write((sbyte)target);
            packet.Send();
            return -1;
        }

        /// <summary>
        /// 生成 NPC（Vector2 位置重载）。
        /// </summary>
        public static int SpawnNPC(IEntitySource source, Vector2 position, int type, int start = 0, float ai0 = 0f, float ai1 = 0f, int target = 255)
            => SpawnNPC(source, (int)position.X, (int)position.Y, type, start, ai0, ai1, target);

        /// <summary>
        /// 杀死 NPC。客户端永不直接 Kill，发 <see cref="CoraliteNetWorkEnum.RequestKillNPC"/> 请求。
        /// </summary>
        public static void KillNPC(int whoAmI)
        {
            if (whoAmI < 0 || whoAmI >= Main.maxNPCs)
                return;

            if (VaultUtils.isSinglePlayer || VaultUtils.isServer)
            {
                NPC npc = Main.npc[whoAmI];
                if (!npc.active)
                    return;

                npc.life = 0;
                npc.checkDead();
                if (VaultUtils.isServer)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, whoAmI);

                return;
            }

            ModPacket packet = Coralite.Instance.GetPacket();
            packet.Write((byte)CoraliteNetWorkEnum.RequestKillNPC);
            packet.Write((short)whoAmI);
            packet.Send();
        }

        /// <summary>
        /// 生成物品并同步（复用 InnoVault <see cref="VaultUtils.SpwanItem"/>）。
        /// </summary>
        public static int SpawnItem(IEntitySource source, Vector2 position, Item item, bool netUpdate = true)
            => VaultUtils.SpwanItem(source, position, item, netUpdate);

        /// <summary>
        /// 生成物品（带随机偏移区域）。
        /// </summary>
        public static int SpawnItem(IEntitySource source, Vector2 position, Vector2 randomSize, Item item, bool netUpdate = true)
            => VaultUtils.SpwanItem(source, position, randomSize, item, netUpdate);

        /// <summary>
        /// 生成物品（矩形区域）。
        /// </summary>
        public static int SpawnItem(IEntitySource source, Rectangle area, Item item, bool netUpdate = true)
            => VaultUtils.SpwanItem(source, area, item, netUpdate);

        internal static void ServerHandleSpawnRequest(BinaryReader reader, int fromWho)
        {
            if (!VaultUtils.isServer)
                return;

            EnsureWhitelistInitialized();

            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int type = reader.ReadInt32();
            byte start = reader.ReadByte();
            float ai0 = reader.ReadSingle();
            float ai1 = reader.ReadSingle();
            sbyte target = reader.ReadSByte();

            if (fromWho < 0 || fromWho >= Main.maxPlayers || !Main.player[fromWho].active)
                return;

            if (!ClientSpawnWhitelist.Contains(type))
            {
                Coralite.Instance.Logger.Warn($"RequestSpawnNPC: 拒绝未白名单 type {type} from {fromWho}");
                return;
            }

            Player player = Main.player[fromWho];
            if (Vector2.Distance(player.Center, new Vector2(x, y)) > MaxSpawnDistanceFromPlayer)
            {
                Coralite.Instance.Logger.Warn($"RequestSpawnNPC: 距离过远 type {type} from {fromWho}");
                return;
            }

            int elapsed = (int)(Main.GameUpdateCount - lastSpawnRequestTick[fromWho]);
            if (elapsed < SpawnRequestCooldownTicks)
                return;

            lastSpawnRequestTick[fromWho] = (int)Main.GameUpdateCount;

            IEntitySource source = player.GetSource_FromThis();
            SpawnNPC(source, x, y, type, start, ai0, ai1, target);
        }

        internal static void ServerHandleKillRequest(BinaryReader reader, int fromWho)
        {
            if (!VaultUtils.isServer)
                return;

            EnsureWhitelistInitialized();

            int whoAmI = reader.ReadInt16();

            if (fromWho < 0 || fromWho >= Main.maxPlayers || !Main.player[fromWho].active)
                return;

            if (whoAmI < 0 || whoAmI >= Main.maxNPCs)
                return;

            NPC npc = Main.npc[whoAmI];
            if (!npc.active)
                return;

            if (!ClientKillWhitelist.Contains(npc.type))
            {
                Coralite.Instance.Logger.Warn($"RequestKillNPC: 拒绝未白名单 type {npc.type} from {fromWho}");
                return;
            }

            int elapsed = (int)(Main.GameUpdateCount - lastKillRequestTick[fromWho]);
            if (elapsed < KillRequestCooldownTicks)
                return;

            lastKillRequestTick[fromWho] = (int)Main.GameUpdateCount;

            KillNPC(whoAmI);
        }
    }
}
