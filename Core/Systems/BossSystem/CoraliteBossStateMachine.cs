using InnoVault.StateMachines;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Coralite Boss 专用 <see cref="NpcStateMachine{TContext}"/>，固定使用 <c>ai[0]</c> 同步顶层状态 ID。
    /// </summary>
    public sealed class CoraliteBossStateMachine<TContext> : NpcStateMachine<TContext>
        where TContext : CoraliteBossContext
    {
        public CoraliteBossStateMachine(TContext context)
            : base(context, CoraliteBossContext.StateAiSlot)
        {
        }
    }
}
