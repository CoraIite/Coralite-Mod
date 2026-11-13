using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.P1Dizziness, typeof(ShadowBallContext))]
    public sealed class P1Dizziness : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.P1Dizziness;

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocitySlowDown(0.95f);

            if (Timer==1)//眩晕粒子
            {

            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (Timer <= 60*5)
                return null;

            ShadowBallStates next = ctx.Boss.GetSmallBalls() == 0
                ? ShadowBallStates.SummonSmallShdowBall
                : ShadowBallStates.RedShift;
            return ShadowBallHubState.CommitTest(ctx, next);
        }
    }
}
