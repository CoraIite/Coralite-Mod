using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    /// <summary>
    /// 史莱姆皇帝顶层招式 FSM 基类（包壳法）：<br/>
    /// 在 <see cref="OnEnter"/> 走基座清空 SonState/Timer 并刷新攻击随机源，再统一做旧 <c>ResetStates</c> 的招前清理（双端），<br/>
    /// 招式 body 仍走旧 partial 方法；状态 ID 经 ai[0] 同步。
    /// </summary>
    public abstract class SlimeEmperorState : CoraliteBossState<SlimeEmperorContext>
    {
        protected SlimeEmperor Boss { get; private set; }

        public override void OnEnter(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            Boss = ctx.Boss;
            base.OnEnter(machine, ctx);     // ResetAttackLocals(SonState/Timer=0) + RollAttackSeed
            ctx.Boss.OnAttackEnter();       // 招前公共清理 + 起跳判定（双端）
        }

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
            => Boss = ctx.Boss;

        protected override IVaultState<SlimeEmperorContext> ServerUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            Boss = ctx.Boss;
            return null;
        }
    }

    /// <summary>招式包壳态：双端调用旧招式方法。</summary>
    public abstract class SlimeEmperorAttackWrapperState : SlimeEmperorState
    {
        protected abstract void RunAttack(SlimeEmperor boss);

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            RunAttack(ctx.Boss);
        }
    }

    [VaultState((int)SlimeEmperor.AIStates.GelShoot, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorGelShootState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.GelShoot();
    }

    [VaultState((int)SlimeEmperor.AIStates.CrownStrike, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorCrownStrikeState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.CrownStrike();
    }

    [VaultState((int)SlimeEmperor.AIStates.SpikeGelBall, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorSpikeGelBallState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.SpikeGelBall();
    }

    [VaultState((int)SlimeEmperor.AIStates.PolymerizeShot, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorPolymerizeShotState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.PolymerizeShot();
    }

    [VaultState((int)SlimeEmperor.AIStates.BodySlam, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorBodySlamState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.BodySlam();
    }

    [VaultState((int)SlimeEmperor.AIStates.Split, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorSplitState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.Split();
    }

    [VaultState((int)SlimeEmperor.AIStates.GelFlippy, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorGelFlippyState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.GelFlippy();
    }

    [VaultState((int)SlimeEmperor.AIStates.StickyGel, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorStickyGelState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.StickyGel();
    }

    [VaultState((int)SlimeEmperor.AIStates.TransportSplit, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorTransportSplitState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.TransportSplit();
    }

    [VaultState((int)SlimeEmperor.AIStates.MiniJump, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorMiniJumpState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.ThreeMiniJump();
    }

    [VaultState((int)SlimeEmperor.AIStates.BigJump, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorBigJumpState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.BigJump();
    }

    [VaultState((int)SlimeEmperor.AIStates.HealGelBall, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorHealGelBallState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.HealGelBall();
    }

    /// <summary>当前无入场动画逻辑且无状态转移指向此 ID；进入时服务端 pick 一次后立刻切走，避免每帧 ResetStates。</summary>
    [VaultState((int)SlimeEmperor.AIStates.OnSpawnAnim, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorOnSpawnAnimState : SlimeEmperorState
    {
        public override void OnEnter(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            base.OnEnter(machine, ctx);
            if (!VaultUtils.isClient)
                ctx.Boss.ResetStates();
        }
    }

    [VaultState((int)SlimeEmperor.AIStates.OnKillAnim, typeof(SlimeEmperorContext))]
    public sealed class SlimeEmperorOnKillAnimState : SlimeEmperorAttackWrapperState
    {
        protected override void RunAttack(SlimeEmperor boss) => boss.OnKillAnim();
    }
}
