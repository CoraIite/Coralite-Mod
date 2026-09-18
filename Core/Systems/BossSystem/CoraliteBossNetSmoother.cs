using Terraria;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// 客户端位置纠偏器，替代原版 <see cref="NPC.netOffset"/> 平滑（C6 / C7）。<br/>
    /// 原版平滑把每次快照的位置差累进 <c>netOffset</c>，每帧只放掉 2～4 px、上限 300。Boss 战里快照由命中驱动
    /// （服务端每挨一下就 <c>netUpdate</c>，节流后约每 4～5 帧一包），而两端帧相位天然有 ±1 帧抖动，
    /// 每包都带着一帧位移量级的位置差：巡航 6 px 无所谓，冲刺 10～22 px 就是每包一脚，放不完就叠成锯齿。<br/>
    /// 这里的做法：清掉 <c>netOffset</c>；收包时先用调用方给的帧差把包内位置沿速度投影到本地时钟（抵消相位抖动），
    /// 再与本地上一帧的预测位置比较：差得少就保持本帧连续、之后每帧消化一部分；差得多视为瞬移或真失步，直接认服务端。<br/>
    /// 速度永远取包内值，纠偏只动位置。
    /// </summary>
    public sealed class CoraliteBossNetSmoother
    {
        /// <summary>
        /// 超过此距离视为瞬移 / 真失步，硬对齐不平滑。取 160：约 10 格，比任何一帧位移都大一个量级，
        /// 又小于瞬移类招式的最短位移，两类误差不会混判（沿用 PlanteraNetSmoother 双客户端实证值，2026-09-07）。
        /// </summary>
        public const float SnapDistance = 160f;

        /// <summary>
        /// 每帧消化待纠偏量的比例。0.25 → 四帧还掉约 68%，正好落在两包之间（≈ 4～5 帧），
        /// 下一包到达时上一包的误差基本吃完、不会累积；再大会在低速巡航时肉眼可见地拽一下。
        /// </summary>
        public const float Rate = 0.25f;

        /// <summary>小于 1 px 的残量一次还清，不再按比例无限逼近。</summary>
        private const float SettleDistanceSq = 1f;

        private Vector2 predictedNext;
        private bool hasPrediction;
        /// <summary>尚未消化的纠偏量（服务端位置 − 本地位置）。</summary>
        private Vector2 pending;

        /// <summary>客户端 AI 开头：清原版平滑偏移，分摊一份待消化纠偏。之后的所有读位置都基于纠偏后的坐标。</summary>
        public void BeginClientFrame(NPC npc)
        {
            npc.netOffset = Vector2.Zero;
            if (pending == Vector2.Zero)
            {
                return;
            }

            Vector2 step = pending.LengthSquared() < SettleDistanceSq ? pending : pending * Rate;
            npc.position += step;
            pending -= step;
        }

        /// <summary>客户端 AI 末尾：记下按本地积分预测的下一帧位置（原版随后执行 <c>position += velocity</c>）。</summary>
        public void EndClientFrame(NPC npc)
        {
            predictedNext = npc.position + npc.velocity;
            hasPrediction = true;
        }

        /// <summary>
        /// 收包时刻（<c>ReceiveExtraAI</c> 末尾，此时 position / velocity 已被服务端值覆盖）。<br/>
        /// <paramref name="frameDelta"/> = 本地帧数 − 包内帧数（钳 ±2），无法判定时传 0。
        /// 晚到一帧的包在客户端看来 delta = +1：包内位置已过时一帧，沿速度往前推一帧。
        /// </summary>
        public void OnSnapshot(NPC npc, int frameDelta)
        {
            npc.netOffset = Vector2.Zero;
            if (!hasPrediction)
            {
                return;
            }

            Vector2 serverNow = npc.position + (npc.velocity * frameDelta);
            Vector2 error = serverNow - predictedNext;

            if (error.LengthSquared() > SnapDistance * SnapDistance)
            {
                npc.position = serverNow;
                pending = Vector2.Zero;
                return;
            }

            npc.position = predictedNext;
            pending = error;
        }

        /// <summary>丢弃预测与待纠偏量（出生 / 重建时用）。</summary>
        public void Reset()
        {
            predictedNext = Vector2.Zero;
            hasPrediction = false;
            pending = Vector2.Zero;
        }
    }
}
