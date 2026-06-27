using InnoVault;
using InnoVault.StateMachines;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Coralite Boss 状态基类：将 <see cref="IVaultState{TContext}.OnUpdate"/> 拆为
    /// <see cref="SharedUpdate"/>（双端）与 <see cref="ServerUpdate"/>（仅权威端），
    /// 避免生成/切状态逻辑在客户端误跑。
    /// </summary>
    public abstract class CoraliteBossState<TContext> : VaultState<TContext>
        where TContext : CoraliteBossContext
    {
        public sealed override IVaultState<TContext> OnUpdate(VaultStateMachine<TContext> machine, TContext ctx)
        {
            SharedUpdate(machine, ctx);

            if (VaultUtils.isClient)
            {
                return null;
            }

            return ServerUpdate(machine, ctx);
        }

        /// <summary>双端执行：动画、粒子、移动插值、帧更新等。</summary>
        protected virtual void SharedUpdate(VaultStateMachine<TContext> machine, TContext ctx) { }

        /// <summary>仅权威端：弹幕生成、子状态推进、返回下一顶层状态。</summary>
        protected virtual IVaultState<TContext> ServerUpdate(VaultStateMachine<TContext> machine, TContext ctx)
            => null;

        public override void OnEnter(VaultStateMachine<TContext> machine, TContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.ResetAttackLocals();
            ctx.RollAttackSeed();
        }
    }
}
