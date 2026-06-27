using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.CoraliteNotes.ConstellationChapter;
using Coralite.Content.CoraliteNotes.NightmareChapter;
using Coralite.Content.CoraliteNotes.SteelChapter;
using Coralite.Content.CoraliteNotes.ThunderChapter1;
using Coralite.Content.UI.NewKnowledgeUnlock;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Network;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.KeySystem
{
    public partial class KnowledgeSystem
    {
        /// <summary>
        /// 服务端或单人：根据 NPC 击杀判定并向对应玩家推送知识解锁。
        /// </summary>
        public static void TryUnlockKnowledgeOnNPCKill(NPC npc)
        {
            int playerIndex = ResolveKillingPlayer(npc);
            if (playerIndex < 0)
                return;

            switch (npc.type)
            {
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                case NPCID.SkeletronPrime:
                case NPCID.TheDestroyer:
                    {
                        bool mechBoss1 = NPC.downedMechBoss1 || npc.type == NPCID.TheDestroyer;
                        bool mechBoss2 = NPC.downedMechBoss2
                            || (npc.type == NPCID.Retinazer && !NPC.AnyNPCs(NPCID.Spazmatism))
                            || (npc.type == NPCID.Spazmatism && !NPC.AnyNPCs(NPCID.Retinazer));
                        bool mechBoss3 = NPC.downedMechBoss3 || npc.type == NPCID.SkeletronPrime;

                        if (Main.hardMode && mechBoss1 && mechBoss2 && mechBoss3)
                            NotifyPlayerUnlock<Thunder1Knowledge>(playerIndex, Coralite.ThunderveinYellow);
                    }
                    break;
                case NPCID.WallofFlesh:
                    NotifyPlayerUnlock<ConstellationKnowledge>(playerIndex, new Color(20, 255, 199));
                    NotifyPlayerUnlock<SteelKnowledge>(playerIndex, Color.LightGray);
                    break;
                case NPCID.MoonLordCore:
                    NotifyPlayerUnlock<NightmareKnowledge>(playerIndex, NightmarePlantera.nightPurple);
                    break;
            }
        }

        private static int ResolveKillingPlayer(NPC npc)
        {
            int playerIndex = npc.lastInteraction;
            if (playerIndex >= 0 && playerIndex < Main.maxPlayers && Main.player[playerIndex].active)
                return playerIndex;

            if (VaultUtils.isSinglePlayer)
                return Main.myPlayer;

            return -1;
        }

        /// <summary>
        /// 向指定玩家推送知识解锁（单人本地 / 服务端发包 / 客户端忽略）。
        /// </summary>
        public static void NotifyPlayerUnlock<T>(int playerIndex, Color color) where T : Knowledge
        {
            Knowledge knowledge = CoraliteContent.GetKnowledge<T>();
            if (VaultUtils.isSinglePlayer)
            {
                if (playerIndex == Main.myPlayer)
                    CheckForUnlock<T>(color);
                return;
            }

            if (VaultUtils.isServer)
                ServerSendUnlockKnowledge(playerIndex, knowledge.InnerType, color);
        }

        /// <summary>
        /// 服务端向指定客户端发送知识解锁包。
        /// </summary>
        public static void ServerSendUnlockKnowledge(int toPlayer, int knowledgeId, Color color)
        {
            if (!VaultUtils.isServer)
                return;

            ModPacket packet = Coralite.Instance.GetPacket();
            packet.Write((byte)CoraliteNetWorkEnum.UnlockKnowledge);
            packet.Write(knowledgeId);
            packet.Write(color.R);
            packet.Write(color.G);
            packet.Write(color.B);
            packet.Write(color.A);
            packet.Send(toPlayer);
        }

        /// <summary>
        /// 客户端处理 <see cref="CoraliteNetWorkEnum.UnlockKnowledge"/> 包。
        /// </summary>
        public static void HandleUnlockKnowledge(BinaryReader reader)
        {
            if (VaultUtils.isServer || Main.dedServ)
                return;

            int knowledgeId = reader.ReadInt32();
            Color color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            ApplyUnlockKnowledgeLocal(knowledgeId, color);
        }

        /// <summary>
        /// 客户端本地解锁知识并弹窗（不发送网络包）。
        /// </summary>
        public static void ApplyUnlockKnowledgeLocal(int knowledgeId, Color color)
        {
            if (knowledgeId < 0 || knowledgeId >= KnowledgeLoader.KnowledgeCount)
                return;

            Knowledge keyKnowledge = CoraliteContent.GetKnowledge(knowledgeId);
            if (keyKnowledge.Unlock)
                return;

            keyKnowledge.UnlockKnowledgeLocal();
            NewKnowledgeState.AddNewTip(knowledgeId, color);
        }
    }
}
