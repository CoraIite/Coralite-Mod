using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.P1Test, typeof(ShadowBallContext))]
    public sealed class P1Test : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.P1Test;

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
            if (Timer <= 60)
                return null;
            //return ShadowBallHubState.EndAttack(ctx);

            ShadowBallStates next = ShadowBallStates.P1Dizziness;// ctx.Boss.GetSmallBalls() == 0? ShadowBallStates.SummonSmallShdowBall: ShadowBallStates.P1Dizziness;
            return ShadowBallHubState.CommitTest(ctx, next);
        }
    }
}
