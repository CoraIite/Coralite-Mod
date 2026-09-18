using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;
using System;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>
    /// 小影子球 FSM 上下文。<br/><br/>
    /// <b>为什么不继承 <see cref="CoraliteBossContext"/></b>：基座把顶层状态 id 钉死在 <c>ai[0]</c>
    /// （<see cref="CoraliteBossContext.StateAiSlot"/> 是 <c>const 0</c>，<see cref="CoraliteBossStateMachine{TContext}"/>
    /// 也是写死 0 构造的），而小球的 <c>ai[0]</c> 装的是主人索引 —— 它被本体的名册扫描、<c>CheckActive</c>
    /// 以及生成包一起用着，挪不走；四个 <c>ai</c> 槽又刚好被基座约定占满，腾不出第五格。
    /// 所以这里走"从属实体"那一路（与荒雷幻影 / 紫电球 / 梦魇钩爪同口径）：
    /// 自己持状态机（id 在 <c>ai[1]</c>），但把基座的三件套原样复用 ——
    /// 热字段槽 <see cref="CoraliteBossHotSlots"/>、纠偏 / 决策点 / 心跳 <see cref="CoraliteMinionNetSync"/>，
    /// 以及 <c>SharedUpdate</c> / <c>AuthorityUpdate</c> 的拆分。接线形状与本体逐字一致，只是类型不同。<br/><br/>
    /// <b>ai 槽</b>：<c>ai[0]</c> = 主人索引，<c>ai[1]</c> = 顶层状态 id（<see cref="AiSlotNetSync{TContext}"/> 同步）。
    /// 旧代码的 <c>ai[2]</c>=SonState、<c>ai[3]</c>=Recorder、<c>localAI[0..3]</c>=Timer / Recorder2..4 全部搬进热槽，
    /// 这两个 ai 槽现在空着留给后续扩展。
    /// </summary>
    public sealed class SmallShadowBallContext : INpcStateContext
    {
        /// <summary>主人（本体影子球）的 NPC 索引，占用 <c>npc.ai[0]</c>。</summary>
        public const int OwnerAiSlot = 0;

        /// <summary>顶层 FSM 状态 ID，占用 <c>npc.ai[1]</c> 并由 <see cref="AiSlotNetSync{TContext}"/> 同步。</summary>
        public const int StateAiSlot = 1;

        public NPC Npc { get; }
        public SmallShadowBall Ball { get; }

        /// <summary>热字段槽，随 <c>SendExtraAI</c> 过线；含义与基座一致：0 Timer、1 Counter、2 Beat、3 Flags、4..11 自用 A..H。</summary>
        public CoraliteBossHotSlots Hot { get; } = new CoraliteBossHotSlots();

        /// <summary>从属实体的联机接线件：清原版平滑 + 自有纠偏 + 决策点 + 慢频心跳。</summary>
        public CoraliteMinionNetSync Net { get; } = new CoraliteMinionNetSync();

        /// <summary>状态机引用，由 <see cref="SmallShadowBallStateMachine"/> 构造时挂上。</summary>
        public SmallShadowBallStateMachine Machine { get; internal set; }

        public SmallShadowBallContext(SmallShadowBall ball)
        {
            Ball = ball;
            Npc = ball.NPC;
        }

        public ref float OwnerIndex => ref Npc.ai[OwnerAiSlot];

        /// <summary>当前状态的热字段视图；状态机未初始化时为 null。</summary>
        internal SmallShadowBallStateBase CurrentHotState => Machine?.CurrentState as SmallShadowBallStateBase;

        #region 同步事实（旋转激光的分层派活参数）

        /// <summary>自身在本层里的序号。旧 <c>Recorder</c>（ai[3]）。</summary>
        public int OrbitIndex { get; set; }

        /// <summary>自身在第几层。旧 <c>Recorder2</c>（localAI[1]）。</summary>
        public int OrbitLayer { get; set; }

        /// <summary>本层一共几个小球。旧 <c>Recorder3</c>（localAI[2]）。</summary>
        public int OrbitLayerCount { get; set; }

        #endregion

        /// <summary>决策点：权威端标记 <c>netUpdate</c>（客户端调用无效果）。</summary>
        public void MarkDecision() => Net.MarkDecision(Npc);

        /// <summary>
        /// 权威端 <c>SendExtraAI</c>：当前状态先把热字段写进 <see cref="Hot"/>，再整块写出，最后跟三个分层参数。
        /// 与本体的 <c>WriteNet</c> 同形状。
        /// </summary>
        public void WriteNet(BinaryWriter writer)
        {
            if (!VaultUtils.isClient)
            {
                CurrentHotState?.WriteHot(this);
            }

            Hot.Write(writer);
            writer.Write((byte)OrbitIndex);
            writer.Write((byte)OrbitLayer);
            writer.Write((byte)OrbitLayerCount);
        }

        /// <summary>
        /// 客户端 <c>ReceiveExtraAI</c>：先读完保证流对齐，再算帧相位差（必须在收养 Timer 之前），
        /// 然后按 <c>ai[1]</c> 是否与本地状态一致决定立刻收养还是挂起等换态，最后交给纠偏器对账。
        /// </summary>
        public void ReadNet(BinaryReader reader)
        {
            Hot.Read(reader);
            OrbitIndex = reader.ReadByte();
            OrbitLayer = reader.ReadByte();
            OrbitLayerCount = reader.ReadByte();

            SmallShadowBallStateBase state = CurrentHotState;
            bool sameState = state != null && state.StateId == (int)Npc.ai[StateAiSlot];

            int frameDelta = 0;
            if (sameState)
            {
                int delta = state.Timer - (int)Hot[CoraliteBossHotSlots.Timer];
                if (Math.Abs(delta) <= CoraliteBossContext.TimerAdoptTolerance)
                {
                    frameDelta = delta;
                }

                Hot.PendingAdopt = false;
                state.ReadHot(this);
            }
            else
            {
                Hot.PendingAdopt = true;
            }

            Net.OnSnapshot(Npc, frameDelta);
        }

        /// <summary>客户端：上一包带来了换态时，等新状态就位后把热字段收养进去。权威端无事可做。</summary>
        public void ConsumePendingHotAdopt()
        {
            if (!Hot.PendingAdopt)
            {
                return;
            }

            SmallShadowBallStateBase state = CurrentHotState;
            if (state == null || state.StateId != (int)Npc.ai[StateAiSlot])
            {
                return;
            }

            Hot.PendingAdopt = false;
            state.ReadHot(this);
        }
    }

    /// <summary>
    /// 小影子球专用状态机：状态 ID 走 <c>ai[1]</c>（<c>ai[0]</c> 留给主人索引），
    /// 并在客户端被 NetSync 换态的那一刻收养热字段（与 <see cref="CoraliteBossStateMachine{TContext}"/> 同款钩子）。
    /// </summary>
    public sealed class SmallShadowBallStateMachine : NpcStateMachine<SmallShadowBallContext>
    {
        public SmallShadowBallStateMachine(SmallShadowBallContext context)
            : base(context, SmallShadowBallContext.StateAiSlot)
        {
            context.Machine = this;
            OnStateChanged += AdoptHotOnNetSync;
        }

        private void AdoptHotOnNetSync(IVaultState<SmallShadowBallContext> oldState, IVaultState<SmallShadowBallContext> newState, StateChangeReason reason)
        {
            if (!VaultUtils.isClient || reason != StateChangeReason.NetSync)
            {
                return;
            }

            Context.ConsumePendingHotAdopt();
        }
    }
}
