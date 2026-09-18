using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 白天狂暴：一路上飞直到脱战。天亮的判定在主控每帧做，这里只负责天黑后回到战斗。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.rampage, typeof(NightmarePlanteraContext))]
    internal sealed class NPRampageState : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.rampage;

        /// <summary>狂暴没有终点，超时兜底会把它踢回选招口，所以关掉。</summary>
        protected override int TimeoutFrames => int.MaxValue;

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            if (!Main.dayTime)
            {
                return;
            }

            NPC npc = ctx.Npc;
            npc.velocity.Y -= NightmarePlanteraDirector.RampageRiseAccel;
            if (npc.velocity.Y < NightmarePlanteraDirector.RampageMaxRiseSpeed)
            {
                npc.velocity.Y = NightmarePlanteraDirector.RampageMaxRiseSpeed;
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => Main.dayTime ? null : Create(ctx.Boss.ResumeStateId());
    }
}
