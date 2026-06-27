using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 雷龙顶层招式 FSM 基类：<br/>
    /// 在 <see cref="OnEnter"/> 注入 Boss 引用、走基座清空 SonState/Timer 并刷新服务端攻击随机源，<br/>
    /// 同时统一做旧 <c>ResetStates</c> 里的视觉量清理（双端），招式 body 仍走旧 partial 方法（包壳法）。
    /// </summary>
    public abstract class ThunderveinDragonState : CoraliteBossState<ThunderveinDragonContext>
    {
        protected ThunderveinDragon Boss { get; private set; }

        public override void OnEnter(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            Boss = ctx.Boss;
            base.OnEnter(machine, ctx);     // ResetAttackLocals(SonState/Timer=0) + RollAttackSeed(服务端)

            ThunderveinDragon boss = ctx.Boss;
            if (boss.NPC.spriteDirection != boss.oldSpriteDirection)
                boss.NPC.rotation += 3.141f;

            boss.shadowAlpha = 1;
            boss.shadowScale = 1;
            boss.canDrawShadows = false;
            boss.isDashing = false;
            boss.currentSurrounding = false;
            boss.NPC.dontTakeDamage = false;
            boss.Recorder = 0;
            boss.Recorder2 = 0;

            boss.RefreshAttackRandom();     // 从已同步的 AttackSeed 派生
        }

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
            => Boss = ctx.Boss;

        protected override IVaultState<ThunderveinDragonContext> ServerUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            Boss = ctx.Boss;
            return null;
        }
    }

    /// <summary>招式包壳态：双端调用旧招式方法（移动/视觉确定性双端跑，生成内部已服务端守卫）。</summary>
    public abstract class ThunderveinAttackWrapperState : ThunderveinDragonState
    {
        protected abstract void RunAttack(ThunderveinDragon boss);

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            RunAttack(ctx.Boss);
        }
    }

    [VaultState((int)ThunderveinDragon.AIStates.onSpawnAnmi, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinOnSpawnAnimState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.OnSpawnAnmi();
    }

    [VaultState((int)ThunderveinDragon.AIStates.onKillAnim, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinOnKillAnimState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.OnKillAnmi();
    }

    [VaultState((int)ThunderveinDragon.AIStates.SmallDash, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinSmallDashState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss)
        {
            if (boss.Phase == 1)
                boss.SmallDashP1();
            else
                boss.SmallDashP2();
        }
    }

    [VaultState((int)ThunderveinDragon.AIStates.LightningRaid, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinLightningRaidState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss)
        {
            if (boss.Phase == 1)
                boss.LightningRaidP1();
            else
                boss.LightningRaidP2();
        }
    }

    [VaultState((int)ThunderveinDragon.AIStates.Discharging, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinDischargingState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.Discharging();
    }

    [VaultState((int)ThunderveinDragon.AIStates.LightningBreath, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinLightningBreathState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss)
        {
            if (boss.Phase == 1)
                boss.LightingBreathP1();
            else
                boss.LightingBreathP2();
        }
    }

    [VaultState((int)ThunderveinDragon.AIStates.LightningBall, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinLightningBallState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.LightingBall();
    }

    [VaultState((int)ThunderveinDragon.AIStates.CrossLightingBall, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinCrossLightingBallState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.CrossLightingBall();
    }

    [VaultState((int)ThunderveinDragon.AIStates.FallingThunder, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinFallingThunderState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss)
        {
            if (boss.Phase == 1)
                boss.FallingThunderP1();
            else
                boss.FallingThunderP2();
        }
    }

    [VaultState((int)ThunderveinDragon.AIStates.ExchangeP1_P2, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinExchangeP1P2State : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.ExchangeP1_P2();
    }

    [VaultState((int)ThunderveinDragon.AIStates.DashDischarging, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinDashDischargingState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.DashDischarging();
    }

    [VaultState((int)ThunderveinDragon.AIStates.GravitationThunder, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinGravitationThunderState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.GravitationThunder();
    }

    [VaultState((int)ThunderveinDragon.AIStates.ElectromagneticCannon, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinElectromagneticCannonState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.ElectromagneticCannon();
    }

    [VaultState((int)ThunderveinDragon.AIStates.StygianThunder, typeof(ThunderveinDragonContext))]
    public sealed class ThunderveinStygianThunderState : ThunderveinAttackWrapperState
    {
        protected override void RunAttack(ThunderveinDragon boss) => boss.StygianThunder();
    }
}
