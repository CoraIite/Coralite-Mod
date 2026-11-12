using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>
    /// 小影子球 状态机记录器<br/><br/>
    /// 自己持状态机（id 在 <c>ai[1]</c>），
    /// ai[0] 主人索引，ai[1] 顶层状态 id（<see cref="AiSlotNetSync{TContext}"/> 同步）。
    /// </summary>
    public sealed class SmallShadowBallContext : INpcStateContext
    {
        public const int StateAiSlot = 1;

        public NPC Npc { get; }
        public SmallShadowBall Ball { get; }

        public BossSlots Hot { get; } = new BossSlots();

        /// <summary>从属实体的联机接线件：清原版平滑 + 自有纠偏 + 决策点 + 慢频心跳。</summary>
        public CoraliteMinionNetSync Net { get; } = new CoraliteMinionNetSync();

        /// <summary>状态机引用，由 <see cref="SmallShadowBallStateMachine"/> 构造时挂上。</summary>
        public SmallShadowBallStateMachine Machine { get; internal set; }

        public SmallShadowBallContext(SmallShadowBall ball)
        {
            Ball = ball;
            Npc = ball.NPC;
        }

        public ref float OwnerIndex => ref Npc.ai[0];

        /// <summary>当前状态的热字段视图；状态机未初始化时为 null。</summary>
        internal SmallShadowBallStateBase CurrentHotState => Machine?.CurrentState as SmallShadowBallStateBase;

        #region 旋转激光状态所使用的东西

        /// <summary>自身在本层里的序号</summary>
        public float OrbitIndex
        {
            get => Hot[BossSlots.A];
            set => Hot[BossSlots.A] = value;
        }
        /// <summary>自身在第几层</summary>
        public int OrbitLayer
        {
            get => (int)Hot[BossSlots.B];
            set => Hot[BossSlots.B] = value;
        }
        /// <summary>本层一共几个小球</summary>
        public int OrbitLayerCount
        {
            get => (int)Hot[BossSlots.C];
            set => Hot[BossSlots.C] = value;
        }

        public float Recorder1
        {
            get => Hot[BossSlots.D];
            set => Hot[BossSlots.D] = value;
        }
        public float Recorder2
        {
            get => Hot[BossSlots.E];
            set => Hot[BossSlots.E] = value;
        }
        public float Recorder3
        {
            get => Hot[BossSlots.F];
            set => Hot[BossSlots.F] = value;
        }

        #endregion

        /// <summary>决策点：权威端标记 <c>netUpdate</c>（客户端调用无效果）。</summary>
        public void NetUpdate() => Net.NetUpdate(Npc);

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
        }

        public void ReadNet(BinaryReader reader)
        {
            Hot.Read(reader);

            SmallShadowBallStateBase state = CurrentHotState;
            bool sameState = state != null && state.StateId == (int)Npc.ai[StateAiSlot];

            int frameDelta = 0;
            if (sameState)
            {
                int delta = state.Timer - (int)Hot[BossSlots.Timer];
                if (Math.Abs(delta) <= CoraliteBossContext.TimerAdoptTolerance)
                    frameDelta = delta;

                Hot.PendingAdopt = false;
                state.ReadHot(this);
            }
            else
                Hot.PendingAdopt = true;

            Net.OnSnapshot(Npc, frameDelta);
        }

        /// <summary>客户端：上一包带来了换态时，等新状态就位后把热字段收养进去。权威端无事可做。</summary>
        public void ConsumePendingHotAdopt()
        {
            if (!Hot.PendingAdopt)
                return;

            SmallShadowBallStateBase state = CurrentHotState;
            if (state == null || state.StateId != (int)Npc.ai[StateAiSlot])
                return;

            Hot.PendingAdopt = false;
            state.ReadHot(this);
        }

        /// <summary>小球发射激光的前摇时长</summary>
        public int LaserChannelTime()
            => Helper.ScaleValueForDiffMode(60, 60, 50, 40);

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
