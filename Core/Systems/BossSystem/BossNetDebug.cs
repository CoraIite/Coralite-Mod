using InnoVault;
using Terraria;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// 多人 Boss netcode 临时调试日志工具。所有输出统一带 <c>[BMP]</c> 前缀，方便 grep。<br/>
    /// 排查完成后把 <see cref="Enabled"/> 置为 false（或整体删除本文件与各处调用）即可。<br/>
    /// 日志落在各进程自己的 tModLoader 日志里：客户端 <c>client.log</c>、专用服 <c>server.log</c>。
    /// </summary>
    public static class BossNetDebug
    {
        /// <summary>总开关。排查结束后置 false 即可静默。</summary>
        public static bool Enabled = true;

        /// <summary>每隔多少帧打一次周期快照（TICK）。15 = 每秒约 4 次。</summary>
        public static uint TickInterval = 15;

        /// <summary>当前进程标识：SV=专用服 / C{index}=客户端 / SP=单机。</summary>
        public static string Side
        {
            get
            {
                if (Main.dedServ)
                    return "SV";
                if (VaultUtils.isClient)
                    return "C" + Main.myPlayer;
                return "SP";
            }
        }

        public static void Log(string tag, string msg)
        {
            if (!Enabled)
                return;

            Coralite.Instance?.Logger.Info($"[BMP][{Side}][f{Main.GameUpdateCount}] {tag} {msg}");
        }

        /// <summary>周期快照节流判断。</summary>
        public static bool ShouldTick => Enabled && TickInterval > 0 && Main.GameUpdateCount % TickInterval == 0;

        public static string Npc(NPC npc)
        {
            if (npc == null)
                return "npc=null";

            return $"npc#{npc.whoAmI} act={(npc.active ? 1 : 0)} hp={npc.life} " +
                   $"ai0={npc.ai[0]:0.#} seed1={npc.ai[1]:0} son2={npc.ai[2]:0.#} timer3={npc.ai[3]:0.#} " +
                   $"pos=({npc.Center.X:0},{npc.Center.Y:0}) vel=({npc.velocity.X:0.0},{npc.velocity.Y:0.0}) " +
                   $"tgt={npc.target} nu={(npc.netUpdate ? 1 : 0)}";
        }

        public static string Proj(Projectile p)
        {
            if (p == null)
                return "proj=null";

            return $"proj#{p.whoAmI} type={p.type} act={(p.active ? 1 : 0)} owner={p.owner} " +
                   $"ai0={p.ai[0]:0.#} ai1={p.ai[1]:0} ai2={p.ai[2]:0.#} " +
                   $"pos=({p.Center.X:0},{p.Center.Y:0}) tl={p.timeLeft}";
        }
    }
}
