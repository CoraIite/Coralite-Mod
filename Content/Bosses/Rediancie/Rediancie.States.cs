using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.Rediancie
{
    /// <summary>
    /// 赤玉灵顶层 FSM 状态基类（包壳法）：招式 body 仍调用旧方法，在两端运行（移动/视觉/弹药环绕），
    /// 生成与选招逻辑在方法内部按服务端守卫处理。<br/>
    /// 这里<b>不</b>调用基座 <see cref="CoraliteBossState{TContext}.OnEnter"/>：Rediancie 的 DamageCount(ai[1])/MoveCyclingType(ai[2])/
    /// OwnedFollowersCount(ai[3]) 仍占用同步 ai 槽，若走基座 ResetAttackLocals/RollAttackSeed 会清零/覆写这些载荷。
    /// </summary>
    public abstract class RediancieBossState : CoraliteBossState<RediancieContext>
    {
        public override void OnEnter(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            // 故意留空：见类注释，Timer/弹药等由招式方法与 ResetState 自身管理。
        }

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            RunBody(ctx.Boss);
        }

        protected abstract void RunBody(Rediancie boss);
    }

    [VaultState((int)Rediancie.AIStates.onSpawnAnim, typeof(RediancieContext))]
    public sealed class RediancieOnSpawnAnimState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.OnSpawnAnim();
    }

    [VaultState((int)Rediancie.AIStates.onKillAnim, typeof(RediancieContext))]
    public sealed class RediancieOnKillAnimState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.OnKillAnim();
    }

    [VaultState((int)Rediancie.AIStates.pulse, typeof(RediancieContext))]
    public sealed class RedianciePulseState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.Pulse();
    }

    [VaultState((int)Rediancie.AIStates.firework, typeof(RediancieContext))]
    public sealed class RediancieFireworkState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.Firework();
    }

    [VaultState((int)Rediancie.AIStates.accumulate, typeof(RediancieContext))]
    public sealed class RediancieAccumulateState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.Accumulate();
    }

    [VaultState((int)Rediancie.AIStates.dash, typeof(RediancieContext))]
    public sealed class RediancieDashState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.Dash();
    }

    [VaultState((int)Rediancie.AIStates.explosion, typeof(RediancieContext))]
    public sealed class RediancieExplosionState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.Explosion();
    }

    [VaultState((int)Rediancie.AIStates.upShoot, typeof(RediancieContext))]
    public sealed class RediancieUpShootState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.UpShoot();
    }

    [VaultState((int)Rediancie.AIStates.magicShoot, typeof(RediancieContext))]
    public sealed class RediancieMagicShootState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.MagicShoot();
    }

    [VaultState((int)Rediancie.AIStates.summon, typeof(RediancieContext))]
    public sealed class RediancieSummonState : RediancieBossState
    {
        protected override void RunBody(Rediancie boss) => boss.Summon();
    }
}
