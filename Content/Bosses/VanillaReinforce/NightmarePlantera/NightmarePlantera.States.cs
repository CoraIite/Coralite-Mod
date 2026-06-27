using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.BossSystem;
using InnoVault;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 梦魇世纪花顶层 FSM：宏观梦境阶段包壳，招式 body 仍走旧 partial 方法。
    /// 状态 ID 与 <see cref="NightmarePlantera.AIPhases"/> 数值一致，占用 ai[0] 同步。
    /// </summary>
    public abstract class NightmarePlanteraBossState : CoraliteBossState<NightmarePlanteraContext>
    {
        protected NightmarePlantera Boss { get; private set; }

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            Boss = ctx.Boss;
            // 选招种子仅由 RollAttackSeedForNextMove 控制，跳过基座 OnEnter 的 RollAttackSeed。
            ctx.ResetAttackLocals();
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => Boss = ctx.Boss;

        protected override IVaultState<NightmarePlanteraContext> ServerUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            Boss = ctx.Boss;
            return null;
        }
    }

    public abstract class NightmarePlanteraPhaseWrapperState : NightmarePlanteraBossState
    {
        protected abstract void RunPhase(NightmarePlantera boss);

        protected virtual bool IncrementTimer => false;

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            RunPhase(ctx.Boss);
            if (IncrementTimer)
            {
                ctx.Boss.Timer++;
            }
        }
    }

    [VaultState((int)NightmarePlantera.AIPhases.OnSpawnAnmi_P0, typeof(NightmarePlanteraContext))]
    public sealed class NightmarePlanteraOnSpawnState : NightmarePlanteraPhaseWrapperState
    {
        protected override void RunPhase(NightmarePlantera boss) => boss.OnSpawnAnmi();
    }

    [VaultState((int)NightmarePlantera.AIPhases.Sleeping_P1, typeof(NightmarePlanteraContext))]
    public sealed class NightmarePlanteraP1SleepingState : NightmarePlanteraPhaseWrapperState
    {
        protected override void RunPhase(NightmarePlantera boss) => boss.Sleeping_Phase1();
    }

    [VaultState((int)NightmarePlantera.AIPhases.Exchange_P1_P2, typeof(NightmarePlanteraContext))]
    public sealed class NightmarePlanteraExchangeP1P2State : NightmarePlanteraPhaseWrapperState
    {
        protected override void RunPhase(NightmarePlantera boss) => boss.Exchange_P1_P2();
    }

    [VaultState((int)NightmarePlantera.AIPhases.Dream_P2, typeof(NightmarePlanteraContext))]
    public sealed class NightmarePlanteraDreamP2State : NightmarePlanteraPhaseWrapperState
    {
        protected override void RunPhase(NightmarePlantera boss) => boss.Dream_Phase2();
    }

    [VaultState((int)NightmarePlantera.AIPhases.Nightemare_P3, typeof(NightmarePlanteraContext))]
    public sealed class NightmarePlanteraNightmareP3State : NightmarePlanteraPhaseWrapperState
    {
        private bool appliedClientExchangeCamera;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            appliedClientExchangeCamera = false;
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            if (!Main.dedServ && !appliedClientExchangeCamera)
            {
                if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera nCamera))
                {
                    nCamera.useShake = false;
                    nCamera.useScreenMove = false;
                }

                appliedClientExchangeCamera = true;
            }

            base.SharedUpdate(machine, ctx);
        }

        protected override void RunPhase(NightmarePlantera boss) => boss.Nightmare_Phase3();
    }

    [VaultState((int)NightmarePlantera.AIPhases.Rampage, typeof(NightmarePlanteraContext))]
    public sealed class NightmarePlanteraRampageState : NightmarePlanteraPhaseWrapperState
    {
        protected override void RunPhase(NightmarePlantera boss) => boss.Rampage();
    }

    [VaultState((int)NightmarePlantera.AIPhases.SuddenDeath, typeof(NightmarePlanteraContext))]
    public sealed class NightmarePlanteraSuddenDeathState : NightmarePlanteraPhaseWrapperState
    {
        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            if (!Main.dedServ)
            {
                ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;
                ctx.Boss.NormallySetTentacle();
            }
        }

        protected override void RunPhase(NightmarePlantera boss)
        {
            boss.SuddenDeath();
            boss.NormallyUpdateTentacle();
        }
    }
}
