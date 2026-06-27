using Coralite.Core.Systems.BossSystem;
using InnoVault;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>影子球顶层 FSM 状态 ID，与 <see cref="VaultStateAttribute"/> 注册 ID 一致，并占用 <c>npc.ai[0]</c> 同步。</summary>
    public enum ShadowBallStateId
    {
        OnSpawnAnim = 0,
        Rampage = 1,
        RollingLaser = 2,
        ConvergeLaser = 3,
        LaserWithBeam = 4,
        LeftRightLaser = 5,
        RollingShadowPlayer = 6,
        RandomLaser = 7,
        P1ToP2Exchange = 8,
        SmashDown = 9,
        VerticalRolling = 10,
        SkyJump = 11,
        HorizontalDash = 12,
        NightmareKingDash = 13,
    }

    /// <summary>
    /// 影子球状态基类：服务端在 <see cref="OnEnter"/> 注入 Boss 引用、重置招式局部量与刷新攻击随机源；<br/>
    /// 客户端 NetSync 被动切状态时<b>不</b>清零已同步的 SonState/Timer 与 localAI Recorder，避免多人偏差。<br/>
    /// 招式 body 仍走旧的 partial 方法（包壳法），但生成逻辑全部下沉到服务端守卫。
    /// </summary>
    public abstract class ShadowBallBossState : CoraliteBossState<ShadowBallContext>
    {
        protected ShadowBall Boss { get; private set; }

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            Boss = ctx.Boss;
            if (VaultUtils.isClient)
            {
                return;
            }

            base.OnEnter(machine, ctx);     // VaultState 计数器清零 + ResetAttackLocals + RollAttackSeed
            Boss.Recorder = 0;
            Boss.Recorder2 = 0;
            Boss.RefreshAttackRandom();
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            Boss = ctx.Boss;
        }

        protected override IVaultState<ShadowBallContext> ServerUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            Boss = ctx.Boss;
            return null;
        }
    }

    /// <summary>
    /// 招式包壳态：在两端调用旧的招式方法（移动/视觉确定性双端跑，生成内部已服务端守卫），<br/>
    /// 招式推进/切状态由方法内的 <see cref="ShadowBall.CompleteCurrentAttack"/>（服务端）驱动。<br/>
    /// <see cref="IncrementTimer"/> 与 <see cref="UpdateCaches"/> 精确对齐旧 AI() switch 中各状态的调用方式，避免单机退化。
    /// </summary>
    public abstract class ShadowBallAttackWrapperState : ShadowBallBossState
    {
        protected abstract void RunAttack(ShadowBall boss);

        /// <summary>是否每帧自增 <c>Timer</c>（倒计时型招式由方法内部自行 <c>Timer--</c>，此处返回 <see langword="false"/>）。</summary>
        protected virtual bool IncrementTimer => true;

        /// <summary>是否在招式后调用 <see cref="ShadowBall.UpdateCachesNormally"/>（部分招式在方法内部自行更新拖尾）。</summary>
        protected virtual bool UpdateCaches => true;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            RunAttack(ctx.Boss);
            if (UpdateCaches)
            {
                ctx.Boss.UpdateCachesNormally();
            }
            if (IncrementTimer)
            {
                ctx.Boss.Timer++;
            }
        }
    }

    [VaultState((int)ShadowBallStateId.OnSpawnAnim, typeof(ShadowBallContext))]
    public sealed class ShadowBallOnSpawnAnimState : ShadowBallBossState
    {
        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            ctx.Boss.OnSpawnAnmi();
            ctx.Boss.Timer++;
        }
    }

    [VaultState((int)ShadowBallStateId.Rampage, typeof(ShadowBallContext))]
    public sealed class ShadowBallRampageState : ShadowBallBossState
    {
        // 旧实现里 Rampage 直接落到 switch 的 default 分支 -> ResetState 重新选招，
        // 这里等价为：服务端立刻挑选下一个招式继续战斗。
        protected override IVaultState<ShadowBallContext> ServerUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            ctx.Boss.NPC.TargetClosest();
            return ctx.Boss.PickNextAttackState();
        }
    }

    [VaultState((int)ShadowBallStateId.RollingLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallRollingLaserState : ShadowBallAttackWrapperState
    {
        protected override void RunAttack(ShadowBall boss) => boss.RollingLaser();
    }

    [VaultState((int)ShadowBallStateId.ConvergeLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallConvergeLaserState : ShadowBallAttackWrapperState
    {
        protected override void RunAttack(ShadowBall boss) => boss.ConvergeLaser();
    }

    [VaultState((int)ShadowBallStateId.LaserWithBeam, typeof(ShadowBallContext))]
    public sealed class ShadowBallLaserWithBeamState : ShadowBallAttackWrapperState
    {
        protected override bool IncrementTimer => false; // 招式内部 Timer--
        protected override void RunAttack(ShadowBall boss) => boss.LaserWithBeam();
    }

    [VaultState((int)ShadowBallStateId.LeftRightLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallLeftRightLaserState : ShadowBallAttackWrapperState
    {
        protected override bool IncrementTimer => false;
        protected override void RunAttack(ShadowBall boss) => boss.LeftRightLaser();
    }

    [VaultState((int)ShadowBallStateId.RollingShadowPlayer, typeof(ShadowBallContext))]
    public sealed class ShadowBallRollingShadowPlayerState : ShadowBallAttackWrapperState
    {
        protected override void RunAttack(ShadowBall boss) => boss.RollingShadowPlayer();
    }

    [VaultState((int)ShadowBallStateId.RandomLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallRandomLaserState : ShadowBallAttackWrapperState
    {
        protected override bool IncrementTimer => false;
        protected override void RunAttack(ShadowBall boss) => boss.RandomLaser();
    }

    [VaultState((int)ShadowBallStateId.P1ToP2Exchange, typeof(ShadowBallContext))]
    public sealed class ShadowBallP1ToP2ExchangeState : ShadowBallBossState
    {
        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Boss.NPC.TargetClosest();
            ctx.Boss.ApplyPhase2Hitbox();           // 两端都改 hitbox
            ctx.Boss.ExchangeToPhase2VisualOnly();  // 仅客户端构造影子玩家
        }

        protected override IVaultState<ShadowBallContext> ServerUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            return VaultStateRegistry<ShadowBallContext>.Create((int)ShadowBallStateId.SmashDown);
        }
    }

    [VaultState((int)ShadowBallStateId.SmashDown, typeof(ShadowBallContext))]
    public sealed class ShadowBallSmashDownState : ShadowBallAttackWrapperState
    {
        protected override void RunAttack(ShadowBall boss) => boss.SmashDown();
    }

    [VaultState((int)ShadowBallStateId.VerticalRolling, typeof(ShadowBallContext))]
    public sealed class ShadowBallVerticalRollingState : ShadowBallAttackWrapperState
    {
        protected override bool UpdateCaches => false; // 招式内部调用 UpdateCachesNormally
        protected override void RunAttack(ShadowBall boss) => boss.VerticalRolling();
    }

    [VaultState((int)ShadowBallStateId.SkyJump, typeof(ShadowBallContext))]
    public sealed class ShadowBallSkyJumpState : ShadowBallAttackWrapperState
    {
        protected override void RunAttack(ShadowBall boss) => boss.SkyJump();
    }

    [VaultState((int)ShadowBallStateId.HorizontalDash, typeof(ShadowBallContext))]
    public sealed class ShadowBallHorizontalDashState : ShadowBallAttackWrapperState
    {
        protected override bool UpdateCaches => false; // 旧 AI() 中该状态不更新拖尾
        protected override void RunAttack(ShadowBall boss) => boss.HorizontalDash();
    }

    [VaultState((int)ShadowBallStateId.NightmareKingDash, typeof(ShadowBallContext))]
    public sealed class ShadowBallNightmareKingDashState : ShadowBallAttackWrapperState
    {
        protected override bool UpdateCaches => false; // 招式内部调用 UpdateCachesNormally
        protected override void RunAttack(ShadowBall boss) => boss.NightmareKingDash();
    }
}
