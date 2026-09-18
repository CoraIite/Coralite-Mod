using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 一阶段待机：跟着钩爪飘、慢慢转头，攒满 180 点就出招。<br/>
    /// 攒的速度受血量与受击影响——所以它是"进度"而不是"帧数"，单独占一个热字段槽。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.p1_Idle, typeof(NightmarePlanteraContext))]
    internal sealed class NPSleepIdleState : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.p1_Idle;

        /// <summary>待机进度（槽 A）。</summary>
        private float progress;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            progress = 0;
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPSleepingP1State.Phase1Tick(ctx);
            ctx.MeleeDamage = true;

            if (progress >= NightmarePlanteraDirector.P1IdleFrames)
            {
                return;
            }

            ctx.DeclareRotationTowardsTarget(0.1f);

            progress++;
            if (ctx.Npc.life < ctx.Npc.lifeMax * NightmarePlanteraDirector.P1Tier1LifeRatio)
            {
                progress++;
            }

            if (ctx.Npc.life < ctx.Npc.lifeMax * NightmarePlanteraDirector.P1Tier2LifeRatio)
            {
                progress++;
            }

            if (ctx.Npc.justHit)
            {
                progress++;
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => progress >= NightmarePlanteraDirector.P1IdleFrames ? NPSleepingP1State.PickAttack(ctx) : null;

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = progress;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            progress = ctx.Hot[CoraliteBossHotSlots.A];
        }
    }
}
