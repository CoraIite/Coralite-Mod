using Coralite.Core.Systems.BossSystem;
using InnoVault;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 紫伏龙顶层 FSM 基类：统一在 <see cref="OnEnter"/> 注入 Boss 引用、刷新服务端攻击随机源、重置招式局部量。<br/>
    /// 招式 body 仍走旧的 partial 方法（包壳法），生成逻辑全部下沉到服务端守卫；
    /// 选招由 <see cref="ZacurrentDragon.PickNextState"/>（仅服务端）裁决，状态 ID 经 <c>ai[0]</c> 自动同步。
    /// </summary>
    public abstract class ZacurrentBossState : CoraliteBossState<ZacurrentDragonContext>
    {
        protected ZacurrentDragon Boss { get; private set; }

        public override void OnEnter(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            Boss = ctx.Boss;
            Boss.RefreshAttackRandom();

            if (VaultUtils.isClient)
            {
                // 客户端经 ai[0] 被动同步：不能清零 SonState/Timer，否则与服务端招式进度冲突。
                return;
            }

            base.OnEnter(machine, ctx);   // VaultState 计数器清零 + ResetAttackLocals(ai[2]/ai[3]) + RollAttackSeed(服务端)
            Boss.ResetFields();
            OnStateEnter(ctx);
        }

        /// <summary>各状态进入时设置确定性初始值（默认无）。</summary>
        protected virtual void OnStateEnter(ZacurrentDragonContext ctx) { }

        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
            => Boss = ctx.Boss;

        protected override IVaultState<ZacurrentDragonContext> ServerUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            Boss = ctx.Boss;
            return null;
        }
    }

    /// <summary>
    /// 招式包壳态：两端调用旧招式方法（移动/视觉确定性双端跑，生成内部已服务端守卫），<br/>
    /// 招式完成（方法返回 <see langword="true"/>）后由服务端 <see cref="ZacurrentDragon.PickNextState"/> 推进到下一状态。
    /// </summary>
    public abstract class ZacurrentAttackState : ZacurrentBossState
    {
        protected abstract bool RunAttack(ZacurrentDragon boss);

        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            ctx.Boss.AttackFinished = RunAttack(ctx.Boss);
        }

        protected override IVaultState<ZacurrentDragonContext> ServerUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            if (ctx.Boss.AttackFinished)
                return ctx.Boss.PickNextState();

            return null;
        }
    }

    #region 动画 / 特殊状态

    [VaultState((int)ZacurrentDragon.AIStates.Waiting, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentWaitingState : ZacurrentBossState
    {
        protected override IVaultState<ZacurrentDragonContext> ServerUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            ctx.Boss.NPC.TargetClosest();
            return VaultStateRegistry<ZacurrentDragonContext>.Create((int)ZacurrentDragon.AIStates.LightningRaidNormal);
        }
    }

    [VaultState((int)ZacurrentDragon.AIStates.onSpawnAnmi, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentOnSpawnState : ZacurrentBossState
    {
        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            if (ctx.Boss.AttackFinished)
                return;

            ctx.Boss.AttackFinished = ctx.Boss.OnSpawnAnmi();
            if (ctx.Boss.AttackFinished)
                ctx.Boss.ForceRecorder2OnNextLightningRaid = true;
        }

        protected override IVaultState<ZacurrentDragonContext> ServerUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            if (!ctx.Boss.AttackFinished)
                return null;

            ctx.Boss.NPC.netUpdate = true;
            return VaultStateRegistry<ZacurrentDragonContext>.Create((int)ZacurrentDragon.AIStates.LightningRaidNormal);
        }
    }

    [VaultState((int)ZacurrentDragon.AIStates.onKillAnim, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentOnKillState : ZacurrentBossState
    {
        public override void OnEnter(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.OnEnter(machine, ctx);
            // 与旧 CheckDead 对齐的视觉状态（双端），保证客户端进入同步状态后绘制正确。
            ZacurrentDragon boss = ctx.Boss;
            boss.NPC.dontTakeDamage = true;
            boss.currentSurrounding = true;
            boss.canDrawShadows = false;
            boss.IsDashing = false;
        }

        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            ctx.Boss.OnKillAnim();
        }
    }

    [VaultState((int)ZacurrentDragon.AIStates.Break, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentBreakState : ZacurrentBossState
    {
        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            ctx.Boss.AttackFinished = ctx.Boss.BreakState();
        }

        protected override IVaultState<ZacurrentDragonContext> ServerUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            if (ctx.Boss.AttackFinished)
            {
                ctx.Boss.PurpleVolt = false;
                return ctx.Boss.PickNextState();
            }

            return null;
        }
    }

    [VaultState((int)ZacurrentDragon.AIStates.PurpleVoltExchange, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentPurpleVoltExchangeState : ZacurrentBossState
    {
        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            ctx.Boss.AttackFinished = ctx.Boss.PurpleVoltExchange();
        }

        protected override IVaultState<ZacurrentDragonContext> ServerUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            if (ctx.Boss.AttackFinished)
            {
                ctx.Boss.PurpleVolt = true;
                return ctx.Boss.PickNextState();
            }

            return null;
        }
    }

    #endregion

    #region 单招

    [VaultState((int)ZacurrentDragon.AIStates.LightningRaidNormal, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentLightningRaidNormalState : ZacurrentAttackState
    {
        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ctx.Boss.LightningRaidSetStartValue();
        protected override bool RunAttack(ZacurrentDragon boss) => boss.LightningRaidNoraml();
    }

    [VaultState((int)ZacurrentDragon.AIStates.LightningRaidVolt, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentLightningRaidVoltState : ZacurrentAttackState
    {
        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ctx.Boss.LightningRaidSetStartValue();
        protected override bool RunAttack(ZacurrentDragon boss) => boss.LightningRaidVolt();
    }

    [VaultState((int)ZacurrentDragon.AIStates.SmallDash, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentSmallDashState : ZacurrentAttackState
    {
        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ctx.Boss.SmallDashSetStartValue();
        protected override bool RunAttack(ZacurrentDragon boss) => boss.SmallDash<PurpleDash>();
    }

    [VaultState((int)ZacurrentDragon.AIStates.SmallDashVolt, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentSmallDashVoltState : ZacurrentAttackState
    {
        // 旧实现 SmallDashVolt 不调用 SmallDashSetStartValue，Recorder 维持 0（单次短冲，用于身位调整）。
        protected override bool RunAttack(ZacurrentDragon boss) => boss.SmallDash<RedDash>(8, 60);
    }

    [VaultState((int)ZacurrentDragon.AIStates.ElectricBreathSmall, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentElectricBreathSmallState : ZacurrentAttackState
    {
        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ctx.Boss.ElectricBreathSmallSetStartValue();
        protected override bool RunAttack(ZacurrentDragon boss) => boss.ElectricBreathSmall();
    }

    [VaultState((int)ZacurrentDragon.AIStates.ElectricBreathMiddle, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentElectricBreathMiddleState : ZacurrentAttackState
    {
        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ctx.Boss.ElectricBreathMiddleSetStartValue();
        protected override bool RunAttack(ZacurrentDragon boss) => boss.ElectricBreathMiddle();
    }

    [VaultState((int)ZacurrentDragon.AIStates.ElectricBall, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentElectricBallState : ZacurrentAttackState
    {
        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ctx.Boss.ElectricBallSetStartValue();
        protected override bool RunAttack(ZacurrentDragon boss) => boss.ElectricBall();
    }

    [VaultState((int)ZacurrentDragon.AIStates.DashDischarging, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentDashDischargingState : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.DashDischarging();
    }

    [VaultState((int)ZacurrentDragon.AIStates.PointerBall, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentPointerBallState : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.PointerBallP2(120);
    }

    #endregion

    #region 连招

    [VaultState((int)ZacurrentDragon.AIStates.NormalRoarCombo1, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalRoarCombo1State : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.RunNormalRoarCombo1();
    }

    [VaultState((int)ZacurrentDragon.AIStates.NormalRoarCombo2, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalRoarCombo2State : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.RunNormalRoarCombo2();
    }

    [VaultState((int)ZacurrentDragon.AIStates.NormalChainCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalChainComboState : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.RunNormalChainCombo();
    }

    [VaultState((int)ZacurrentDragon.AIStates.NormalPointerCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalPointerComboState : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.RunNormalPointerCombo();
    }

    [VaultState((int)ZacurrentDragon.AIStates.VoltBigCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentVoltBigComboState : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.RunVoltBigCombo();
    }

    [VaultState((int)ZacurrentDragon.AIStates.VoltChainCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentVoltChainComboState : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.RunVoltChainCombo();
    }

    [VaultState((int)ZacurrentDragon.AIStates.VoltZBallChainCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentVoltZBallChainComboState : ZacurrentAttackState
    {
        protected override bool RunAttack(ZacurrentDragon boss) => boss.RunVoltZBallChainCombo();
    }

    #endregion
}
