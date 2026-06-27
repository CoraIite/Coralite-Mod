using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    /// <summary>
    /// 血玉灵顶层 FSM 状态基类（包壳法，与 Rediancie 同模板）：招式 body 仍调用旧方法，在两端运行；
    /// 生成与选招逻辑在方法内部按服务端守卫处理。<br/>
    /// <b>不</b>调用基座 <see cref="CoraliteBossState{TContext}.OnEnter"/>：DamageCount(ai[1])/MoveCyclingType(ai[2])/
    /// OwnedFollowersCount(ai[3]) 仍占用同步 ai 槽，避免被基座 ResetAttackLocals/RollAttackSeed 清零/覆写。
    /// </summary>
    public abstract class BloodiancieBossState : CoraliteBossState<BloodiancieContext>
    {
        public override void OnEnter(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            // 故意留空：见类注释。
        }

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            RunBody(ctx.Boss);
        }

        protected abstract void RunBody(Bloodiancie boss);
    }

    [VaultState((int)Bloodiancie.AIStates.onSpawnAnim, typeof(BloodiancieContext))]
    public sealed class BloodiancieOnSpawnAnimState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.OnSpawnAnim();
    }

    [VaultState((int)Bloodiancie.AIStates.onKillAnim, typeof(BloodiancieContext))]
    public sealed class BloodiancieOnKillAnimState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.OnKillAnim();
    }

    [VaultState((int)Bloodiancie.AIStates.pulse, typeof(BloodiancieContext))]
    public sealed class BloodianciePulseState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.Pulse();
    }

    [VaultState((int)Bloodiancie.AIStates.bloodRain, typeof(BloodiancieContext))]
    public sealed class BloodiancieBloodRainState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.BloodRain();
    }

    [VaultState((int)Bloodiancie.AIStates.firework, typeof(BloodiancieContext))]
    public sealed class BloodiancieFireworkState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.Firework();
    }

    [VaultState((int)Bloodiancie.AIStates.explosionHorizontally, typeof(BloodiancieContext))]
    public sealed class BloodiancieExplosionHorizontallyState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.ExplosionHorizontally();
    }

    [VaultState((int)Bloodiancie.AIStates.dash, typeof(BloodiancieContext))]
    public sealed class BloodiancieDashState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.Dash();
    }

    [VaultState((int)Bloodiancie.AIStates.explosion, typeof(BloodiancieContext))]
    public sealed class BloodiancieExplosionState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.Explosion();
    }

    [VaultState((int)Bloodiancie.AIStates.upShoot, typeof(BloodiancieContext))]
    public sealed class BloodiancieUpShootState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.UpShoot();
    }

    [VaultState((int)Bloodiancie.AIStates.shootBomb, typeof(BloodiancieContext))]
    public sealed class BloodiancieShootBombState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.ShootBomb();
    }

    [VaultState((int)Bloodiancie.AIStates.magicShoot, typeof(BloodiancieContext))]
    public sealed class BloodiancieMagicShootState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.MagicShoot();
    }

    [VaultState((int)Bloodiancie.AIStates.summon, typeof(BloodiancieContext))]
    public sealed class BloodiancieSummonState : BloodiancieBossState
    {
        protected override void RunBody(Bloodiancie boss) => boss.Summon();
    }
}
