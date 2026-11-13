using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.MucuryBurst, typeof(ShadowBallContext))]
    public class MucuryBurst : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.MucuryBurst;

        private enum Beat
        {
            Ready,                                        
            Push,
            Over,
        }

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocitySlowDown(0.95f);
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (Timer <= 60 * 5)
                return null;

            ShadowBallStates next = ctx.Boss.GetSmallBalls() == 0
                ? ShadowBallStates.SummonSmallShdowBall
                : ShadowBallStates.RedShift;
            return ShadowBallHubState.CommitTest(ctx, next);
        }
    }
}
