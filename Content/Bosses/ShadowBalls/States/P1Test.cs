using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>一阶段调试入口：短暂停顿后补球或进入指定测试招式。</summary>
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

            ShadowBallStates next = ctx.Boss.GetSmallBalls() == 0
                ? ShadowBallStates.SummonSmallShdowBall
                : ShadowBallStates.GammaRayBurst;
            return ShadowBallHubState.CommitTest(ctx, next);
        }
    }
}
