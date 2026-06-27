using InnoVault;
using InnoVault.StateMachines;
using Terraria;

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

            IVaultState<TContext> next = ServerUpdate(machine, ctx);

            // 抗抽搐：攻击期间 SonState/Timer 在两端各自本地推进、不持续同步，Boss 高速移动时位置会漂移数百像素，
            // 换招触发整包 netUpdate 时被硬快照拉回 => 视觉抽搐。Boss 移动较快时服务端每帧标记同步，
            // 让客户端持续紧跟服务端的位置/子状态/计时，把漂移压到接近 0。
            if (ctx.Npc.velocity.LengthSquared() > NetSyncSpeedThresholdSq)
            {
                ctx.Npc.netUpdate = true;
            }

            return next;
        }

        /// <summary>触发每帧同步的速度平方阈值（速度 &gt; 6 像素/帧时持续同步）。</summary>
        private const float NetSyncSpeedThresholdSq = 6f * 6f;

        /// <summary>双端执行：动画、粒子、移动插值、帧更新等。</summary>
        protected virtual void SharedUpdate(VaultStateMachine<TContext> machine, TContext ctx) { }

        /// <summary>仅权威端：弹幕生成、子状态推进、返回下一顶层状态。</summary>
        protected virtual IVaultState<TContext> ServerUpdate(VaultStateMachine<TContext> machine, TContext ctx)
            => null;

        public override void OnEnter(VaultStateMachine<TContext> machine, TContext ctx)
        {
            base.OnEnter(machine, ctx);

            // 客户端 NetSync 被动切状态时不能清零 ai[2]/ai[3] 或 roll 新种子，否则会与服务端进度冲突并导致抽搐/阶段卡死。
            if (VaultUtils.isClient)
            {
                return;
            }

            ctx.ResetAttackLocals();
            ctx.RollAttackSeed();
        }
    }
}
