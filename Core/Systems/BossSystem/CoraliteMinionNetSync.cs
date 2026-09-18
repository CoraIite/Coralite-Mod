using System;
using Terraria;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// 从属实体（分身 / 小怪 / 触手 / 钩爪 / 环绕体）的联机接线件，对应契约 C10「部件服从本体契约」。<br/><br/>
    /// 本体走 <see cref="CoraliteBossContext"/>，它把热字段槽、纠偏器、决策点标记打包在一起；从属 NPC 没有 Context，
    /// 也不需要状态机热字段（它们的招式量本来就塞得进 <c>ai[0..3]</c>），缺的只有三件：<br/>
    /// 1. <b>C6</b>——退出原版 <see cref="NPC.netOffset"/> 平滑，与本体保持同一平滑层，否则任何连接两个实体的绘制会在根部脱开；<br/>
    /// 2. <b>C7</b>——自有纠偏，替掉被清掉的原版平滑，避免"只清平滑不配纠偏"反而每包硬跳一次；<br/>
    /// 3. <b>C2</b>——决策点 <c>netUpdate</c> 与慢频心跳兜底。<br/><br/>
    /// 因此这里只是 <see cref="CoraliteBossNetSmoother"/> 的一层薄封装（复用本体同一份纠偏数学与实证参数），
    /// 外加决策点与心跳的门控，以及替代两端各掷一次 <c>Main.rand</c> 的确定性随机源 <see cref="CreateBeatRandom"/>。<br/><br/>
    /// <b>典型接线</b>（从属 <c>ModNPC</c>）：<br/>
    /// <c>AI()</c> 开头 <c>Net.BeginClientFrame(NPC);</c>，结尾 <c>Net.EndClientFrame(NPC); Net.Heartbeat(NPC);</c><br/>
    /// 每个拍点（换态 / 瞬移 / 掷骰 / 出手）<c>Net.MarkDecision(NPC);</c><br/>
    /// <c>ReceiveExtraAI</c> 读完全部字段后 <c>Net.OnSnapshot(NPC);</c><br/><br/>
    /// 位置由每帧硬写决定（如逐帧贴着玩家）的部件不需要纠偏器，只调静态的 <see cref="ClearVanillaSmoothing"/> 即可：
    /// 纠偏量下一帧就会被硬写覆盖，装了也是空转。
    /// </summary>
    public sealed class CoraliteMinionNetSync
    {
        /// <summary>心跳兜底帧数，与本体同口径（<see cref="CoraliteBossContext.DefaultHeartbeatFrames"/>）：心跳只是慢频安全网，真正的快照率由玩家命中率决定。</summary>
        public const int DefaultHeartbeatFrames = CoraliteBossContext.DefaultHeartbeatFrames;

        /// <summary>打散 <c>whoAmI</c> 的大素数，取自常见空间哈希常量；与 <see cref="BeatSeedPrime"/> 互质，避免相邻下标落到相近种子。</summary>
        private const int IndexSeedPrime = 73856093;

        /// <summary>打散拍号的大素数。</summary>
        private const int BeatSeedPrime = 19349663;

        private readonly CoraliteBossNetSmoother smoother = new CoraliteBossNetSmoother();

        private int heartbeatCounter;

        /// <summary>
        /// 只做 C6：清掉原版平滑偏移。<br/>
        /// 原版每帧在 AI 之前按 2～4 px 衰减 <c>netOffset</c>，绘制时再把它加回位置上——于是"AI 里读到的坐标"与"屏幕上画出来的坐标"差了这一截。
        /// 本体已经清零，部件不清就会在连线根部、血条与贴图之间看到这个差值。服务端 <c>netOffset</c> 恒为零，不必调。
        /// </summary>
        public static void ClearVanillaSmoothing(NPC npc)
        {
            if (VaultUtils.isClient)
            {
                npc.netOffset = Vector2.Zero;
            }
        }

        /// <summary>
        /// 由 <c>whoAmI</c> 与拍号确定性派生随机源，替代两端各掷一次的 <c>Main.rand</c>（C1、<c>tml-netcode-pitfalls</c> 9.1）。<br/>
        /// 定种 <see cref="Random"/> 的序列在 .NET 里是跨进程可复现的（基座 <see cref="CoraliteBossContext.CreateAttackRandom"/> 同理），
        /// 所以两端取同一个 <paramref name="beat"/> 就会得到同一串数——分布形状不变，只是不再各掷各的。<br/>
        /// <paramref name="beat"/> 必须是两端都能读到的同步量（轮次计数、已同步的子拍号等）；传常量会让每轮结果一样，那是设计改动。
        /// </summary>
        public static Random CreateBeatRandom(int whoAmI, int beat)
        {
            return new Random(unchecked((whoAmI * IndexSeedPrime) ^ (beat * BeatSeedPrime)));
        }

        /// <summary>客户端 AI 开头：清原版平滑并分摊一份待消化纠偏量。</summary>
        public void BeginClientFrame(NPC npc)
        {
            if (VaultUtils.isClient)
            {
                smoother.BeginClientFrame(npc);
            }
        }

        /// <summary>客户端 AI 末尾：记下本地预测的下一帧位置，供下一次收包对账。</summary>
        public void EndClientFrame(NPC npc)
        {
            if (VaultUtils.isClient)
            {
                smoother.EndClientFrame(npc);
            }
        }

        /// <summary>
        /// 收包时刻（<c>ReceiveExtraAI</c> 把自己的字段全部读完之后，此时 position / velocity / ai[] 已是服务端值）。<br/>
        /// <paramref name="frameDelta"/> = 本地帧数 − 包内帧数，钳 ±2；没有可对照的同步计时器就留 0
        /// （<c>tml-mp-motion-sync</c> REFERENCE §7.2 对无计时器的部件就是这么规定的），此时纠偏仍能吃掉真实失步，只是不抵消帧相位抖动。
        /// </summary>
        public void OnSnapshot(NPC npc, int frameDelta = 0)
        {
            smoother.OnSnapshot(npc, frameDelta);
        }

        /// <summary>
        /// 决策点：权威端标记 <c>netUpdate</c>（客户端调用无效果）。换态、瞬移、掷骰、出手、锁向那一帧调用一次，不要按定时器调用（C2）。
        /// </summary>
        public void MarkDecision(NPC npc)
        {
            if (!VaultUtils.isClient)
            {
                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// 权威端 AI 末尾的慢频兜底心跳，丢包与中途加入靠它自愈。<paramref name="frames"/> ≤ 0 关闭。
        /// </summary>
        public void Heartbeat(NPC npc, int frames = DefaultHeartbeatFrames)
        {
            if (VaultUtils.isClient || frames <= 0)
            {
                return;
            }

            heartbeatCounter++;
            if (heartbeatCounter < frames)
            {
                return;
            }

            heartbeatCounter = 0;
            npc.netUpdate = true;
        }

        /// <summary>丢弃预测与待纠偏量（重建 / 瞬移到新战场时用）。</summary>
        public void Reset()
        {
            smoother.Reset();
            heartbeatCounter = 0;
        }
    }
}
