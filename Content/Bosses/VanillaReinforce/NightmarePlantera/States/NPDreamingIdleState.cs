using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 梦境·待机：七只美梦光凑齐、幻想之神降临之后的空场。boss 原地刹停，把舞台整个让给玩家；
    /// 幻想之神一旦消失（被打掉或超时）立刻回到<b>常规</b>二阶段轮换，而不是继续梦境战斗。<br/>
    /// 原 <c>Dreaming_Idle</c>（Phase.P2_Dream.cs:2053-2071）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.dreaming_Idle, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamingIdleState : NPDreamStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.dreaming_Idle;

        protected override bool DreamingVariant => true;

        /// <summary>整段本来就要等最长 60 秒，超时兜底交给招内的上限，别让基座在 40 秒处截断。</summary>
        protected override int TimeoutFrames => int.MaxValue;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => ctx.DeclareDamp(NightmarePlanteraDirector.P2DreamingIdleDamp);

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            // 幻想之神不在场、或者等满 60 秒：回常规二阶段（旧代码这里调的是 SetPhase2States，不是梦境版）。
            if (!NightmarePlantera.FantasyGodAlive(out _) || Timer > NightmarePlanteraDirector.P2DreamingIdleFrames)
            {
                return NPDreamP2State.Commit(ctx);
            }

            return null;
        }
    }
}
