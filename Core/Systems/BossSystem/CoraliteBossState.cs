using InnoVault.StateMachines;
using System;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Coralite Boss 状态基类：将 <see cref="IVaultState{TContext}.OnUpdate"/> 拆为
    /// <see cref="SharedUpdate"/>（双端）与 <see cref="ServerUpdate"/>（仅权威端），
    /// 避免生成/切状态逻辑在客户端误跑。<br/><br/>
    /// <b>新契约（Phase 0 起）</b><br/>
    /// · 状态在 <see cref="SharedUpdate"/> 里只写 Context 上的声明（运动模式、朝向、表现量）并推进确定性子拍；宿主 <c>ApplyDeclaredMovement</c> 两端落地（C1）。<br/>
    /// · <see cref="ServerUpdate"/> 里只放弹幕生成、掷骰、生命改动与<b>转移</b>——返回值是唯一合法的换态口，招式体内不得调用 <c>ChangeState</c>（客户端调用会本地 OnExit/OnEnter 再被拉回，表现为抽一下）。<br/>
    /// · <see cref="Timer"/> / <see cref="Counter"/> / <see cref="BeatIndex"/> 由 <see cref="WriteHot"/> / <see cref="ReadHot"/> 随 <c>SendExtraAI</c> 过线；子类的自用槽先调 base 再写读（D6）。<br/>
    /// · <see cref="Timer"/> 在本类 OnUpdate <b>开头</b>自增：招式体第一帧读到 1。旧主控的写法是 <c>StateMachine.Update(); Timer++;</c>，
    ///   换态发生在 Update 内部、随后立刻 ++，所以旧招式体第一帧看到的也是 1——保持这个约定，搬过来的 <c>Timer == N</c> 拍点一帧不差。<br/>
    /// · 一次性拍 <c>Timer == N</c> 在客户端收养后可能被跨过：用 <see cref="CueCatchUpGrace"/> 判定“刚越过 → 补放 / 越过很久 → 静默”（C9）。
    /// </summary>
    public abstract class CoraliteBossState<TContext> : VaultState<TContext>, ICoraliteBossHotState
        where TContext : CoraliteBossContext
    {
        /// <summary>
        /// 一次性演出拍的追帧宽限：收养后的 Timer 刚越过拍点不超过这么多帧，视为本机只是慢了半拍，让本地一次性逻辑补放；
        /// 越过更多则是中途加入，静默跳过不补放。20 帧 ≈ 三到四包的间隔，覆盖最坏的相位抖动。
        /// </summary>
        protected const int CueCatchUpGrace = 20;

        /// <summary>
        /// 招式内子拍号（随热字段过线）。子类用私有 <c>enum Beat</c> 映射：<c>(Beat)BeatIndex</c> 读、<see cref="SwitchBeat"/> 写。
        /// 确定性可推导的换拍（按 Timer 到点）两端各自调；掷骰 / 碰撞等只在一端知道的换拍由权威端调，客户端靠收养跟上。
        /// </summary>
        protected int BeatIndex { get; set; }

        public sealed override IVaultState<TContext> OnUpdate(VaultStateMachine<TContext> machine, TContext ctx)
        {
            Timer++;
            SharedUpdate(machine, ctx);

            IVaultState<TContext> next = null;
            if (!VaultUtils.isClient)
            {
                next = ServerUpdate(machine, ctx);

                if (ctx.UseLegacySpeedValve)
                {
                    // 遗留抗抽搐阀（未迁移 boss）：SonState/Timer 在两端各自本地推进、不持续同步，Boss 高速移动时位置会漂移数百像素，
                    // 换招触发整包 netUpdate 时被硬快照拉回 => 视觉抽搐。Boss 移动较快时服务端每帧标记同步把漂移压到接近 0。
                    if (ctx.Npc.velocity.LengthSquared() > NetSyncSpeedThresholdSq)
                    {
                        ctx.Npc.netUpdate = true;
                    }
                }
                else if (ctx.HeartbeatFrames > 0)
                {
                    // 新契约：决策点 MarkDecision + 命中驱动的快照已经够密，这里只是慢频兜底心跳；本帧已有包出门就重新计时。
                    if (ctx.Npc.netUpdate)
                    {
                        ctx.HeartbeatCounter = 0;
                    }
                    else if (++ctx.HeartbeatCounter >= ctx.HeartbeatFrames)
                    {
                        ctx.HeartbeatCounter = 0;
                        ctx.Npc.netUpdate = true;
                    }
                }
            }

            return next;
        }

        /// <summary>遗留阀的速度平方阈值（速度 &gt; 6 像素/帧时持续同步）。</summary>
        private const float NetSyncSpeedThresholdSq = 6f * 6f;

        /// <summary>双端执行：写运动 / 朝向 / 表现声明，推进确定性子拍，客户端粒子音效（<c>!Main.dedServ</c>）。</summary>
        protected virtual void SharedUpdate(VaultStateMachine<TContext> machine, TContext ctx) { }

        /// <summary>仅权威端：弹幕生成、掷骰、生命改动，并<b>返回下一顶层状态</b>（null = 留在本状态）。</summary>
        protected virtual IVaultState<TContext> ServerUpdate(VaultStateMachine<TContext> machine, TContext ctx)
            => null;

        public override void OnEnter(VaultStateMachine<TContext> machine, TContext ctx)
        {
            base.OnEnter(machine, ctx);
            BeatIndex = 0;

            // 客户端 NetSync 被动切状态时不能清零 ai[2]/ai[3] 或 roll 新种子，否则会与服务端进度冲突并导致抽搐/阶段卡死。
            // 客户端只清本地量；Timer/Counter/Beat 随后由收养路径从热字段恢复。
            if (VaultUtils.isClient)
            {
                return;
            }

            ctx.ResetAttackLocals();
            ctx.RollAttackSeed();
        }

        /// <summary>
        /// 换拍：写拍号、Timer 清零、权威端标决策点。一次性拍用 <c>Timer == N</c>，持续拍用区间；别在一个计时器上串烧多拍。
        /// </summary>
        protected void SwitchBeat(TContext ctx, int beat)
        {
            BeatIndex = beat;
            Timer = 0;
            ctx.MarkDecision();
        }

        /// <summary>权威端每包前把热字段写进 <see cref="CoraliteBossContext.Hot"/>；子类先调 base 再写自用槽 A..H。</summary>
        public virtual void WriteHot(TContext ctx)
        {
            ctx.Hot[CoraliteBossHotSlots.Timer] = Timer;
            ctx.Hot[CoraliteBossHotSlots.Counter] = Counter;
            ctx.Hot[CoraliteBossHotSlots.Beat] = BeatIndex;
        }

        /// <summary>
        /// 客户端收包 / 换态后从 <see cref="CoraliteBossContext.Hot"/> 收养热字段；子类先调 base 再读自用槽，派生的一次性标志在此重算。
        /// 可能在目标尚未建立时被调，实现里不要碰目标玩家。
        /// </summary>
        public virtual void ReadHot(TContext ctx)
        {
            Timer = AdoptTimer(Timer, ctx.Hot[CoraliteBossHotSlots.Timer]);
            Counter = (int)ctx.Hot[CoraliteBossHotSlots.Counter];
            BeatIndex = (int)ctx.Hot[CoraliteBossHotSlots.Beat];
        }

        /// <summary>
        /// 计时器收养带容差：本地与服务端只差一两帧是网络抖动常态，硬对齐会让 <c>Timer == X</c> 型一次性拍被跳过或重放；只在真正漂开时才拉齐。
        /// </summary>
        protected static int AdoptTimer(int local, float synced, int tolerance = CoraliteBossContext.TimerAdoptTolerance)
        {
            int server = (int)synced;
            return Math.Abs(server - local) > tolerance ? server : local;
        }

        void ICoraliteBossHotState.WriteHot(CoraliteBossContext ctx) => WriteHot((TContext)ctx);

        void ICoraliteBossHotState.ReadHot(CoraliteBossContext ctx) => ReadHot((TContext)ctx);
    }
}
