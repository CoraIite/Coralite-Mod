using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    /// <summary>
    /// 小冰龙宝宝顶层 FSM 状态基类（包壳法）：<br/>
    /// 招式 body 仍调用旧的 partial 方法，在两端运行（移动/视觉/招内分段），生成与切状态逻辑已在方法内部按服务端守卫处理。<br/>
    /// 这里<b>不</b>调用 <see cref="CoraliteBossState{TContext}.OnEnter"/>：BabyIceDragon 的 Timer(ai[2]) 与 movePhase 由招式方法与
    /// <see cref="BabyIceDragon.ResetStates"/>/<see cref="BabyIceDragon.Dizzy"/>/<see cref="BabyIceDragon.HaveARest"/> 显式管理，
    /// 若走基座 <c>ResetAttackLocals</c> 会在客户端 NetSync 切状态时清零已同步的 Timer，导致回合计时错乱。
    /// </summary>
    public abstract class BabyIceDragonBossState : CoraliteBossState<BabyIceDragonContext>
    {
        public override void OnEnter(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            // 故意留空：见类注释，Timer/movePhase 由招式方法自身管理。
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            RunBody(ctx.Boss);
        }

        protected abstract void RunBody(BabyIceDragon boss);
    }

    [VaultState((int)BabyIceDragon.AIStates.onSpawnAnim, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonOnSpawnAnimState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.OnSpawnAnimBody();
    }

    [VaultState((int)BabyIceDragon.AIStates.roaringAnim, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonRoaringAnimState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.RoaringAnimBody();
    }

    [VaultState((int)BabyIceDragon.AIStates.onKillAnim, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonOnKillAnimState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.OnKillAnimBody();
    }

    [VaultState((int)BabyIceDragon.AIStates.dizzy, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonDizzyState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.DizzyBody();
    }

    [VaultState((int)BabyIceDragon.AIStates.rest, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonRestState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.RestBody();
    }

    [VaultState((int)BabyIceDragon.AIStates.dive, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonDiveState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.Dive();
    }

    [VaultState((int)BabyIceDragon.AIStates.accumulate, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonAccumulateState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.AccumulateBody();
    }

    [VaultState((int)BabyIceDragon.AIStates.iceBreath, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonIceBreathState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.IceBreath();
    }

    [VaultState((int)BabyIceDragon.AIStates.horizontalDash, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonHorizontalDashState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.HorizontalDash();
    }

    [VaultState((int)BabyIceDragon.AIStates.smashDown, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonSmashDownState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.SmashDown();
    }

    [VaultState((int)BabyIceDragon.AIStates.iceThornsTrap, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonIceThornsTrapState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.IceThornsTrap();
    }

    [VaultState((int)BabyIceDragon.AIStates.iceCloud, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonIceCloudState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.IceCloud();
    }

    [VaultState((int)BabyIceDragon.AIStates.doubleDash, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonDoubleDashState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.DoubleDash();
    }

    [VaultState((int)BabyIceDragon.AIStates.iceTornado, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonIceTornadoState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.IceTornado();
    }

    [VaultState((int)BabyIceDragon.AIStates.iciclesFall, typeof(BabyIceDragonContext))]
    public sealed class BabyIceDragonIciclesFallState : BabyIceDragonBossState
    {
        protected override void RunBody(BabyIceDragon boss) => boss.IciclesFall();
    }
}
