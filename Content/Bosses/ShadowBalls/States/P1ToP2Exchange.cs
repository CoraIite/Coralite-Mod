using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.P1ToP2Exchange, typeof(ShadowBallContext))]
    public sealed class P1ToP2Exchange : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.P1ToP2Exchange;

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Npc.TargetClosest();
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocityLetItGo();
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
            => EndAttack(ctx);
    }
}
