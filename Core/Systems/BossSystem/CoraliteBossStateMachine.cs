using InnoVault.StateMachines;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Coralite Boss 专用 <see cref="NpcStateMachine{TContext}"/>，固定使用 <c>ai[0]</c> 同步顶层状态 ID。<br/>
    /// 构造时把自己挂到 <see cref="CoraliteBossContext.Machine"/>，并订阅 <see cref="VaultStateMachine{TContext}.OnStateChanged"/>：
    /// 客户端被 NetSync 换态后新状态实例的 Timer 已被 OnEnter 清零，而带来这次换态的快照里同包装着服务端此刻的计时器
    /// （挂在 <see cref="CoraliteBossContext.Hot"/> 上、<see cref="CoraliteBossHotSlots.PendingAdopt"/> 为真），此处在新状态第一帧 OnUpdate 之前收养。<br/>
    /// <b>EnsureAiMachine 约定</b>：初态从 <c>ai[0]</c> 重建——<c>VaultStateRegistry&lt;TCtx&gt;.Create((int)NPC.ai[0])</c>，返回 null 时回退出生态（中途加入者靠这个不重放入场）。
    /// </summary>
    public sealed class CoraliteBossStateMachine<TContext> : NpcStateMachine<TContext>, ICoraliteBossMachine
        where TContext : CoraliteBossContext
    {
        public CoraliteBossStateMachine(TContext context)
            : base(context, CoraliteBossContext.StateAiSlot)
        {
            context.Machine = this;
            OnStateChanged += AdoptHotOnNetSync;
        }

        /// <inheritdoc/>
        public ICoraliteBossHotState CurrentHotState => CurrentState as ICoraliteBossHotState;

        private void AdoptHotOnNetSync(IVaultState<TContext> oldState, IVaultState<TContext> newState, StateChangeReason reason)
        {
            if (!VaultUtils.isClient || reason != StateChangeReason.NetSync)
            {
                return;
            }

            Context.ConsumePendingHotAdopt();
        }
    }
}
