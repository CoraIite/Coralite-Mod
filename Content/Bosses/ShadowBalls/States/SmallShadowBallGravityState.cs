using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 引力招式占位状态。子拍 0/1 仅作为本体下令的同步标记，具体行为后续补充。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.Gravity, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallGravityState : SmallShadowBallStateBase
    {
        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.Gravity;

        public int GravityBeat => BeatIndex;

        /// <summary>由本体在服务器端切换引力子拍。</summary>
        public void ServerSetBeat(SmallShadowBallContext ctx, int beat)
        {
            if (VaultUtils.isClient || beat is < 0 or > 1)
            {
                return;
            }

            SwitchBeat(ctx, beat);
        }

        protected override void SharedUpdate(
            VaultStateMachine<SmallShadowBallContext> machine,
            SmallShadowBallContext ctx,
            ShadowBall owner)
        {
            // Placeholder: child beats 0 and 1 intentionally have no behavior yet.
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(
            VaultStateMachine<SmallShadowBallContext> machine,
            SmallShadowBallContext ctx,
            ShadowBall owner)
            => null;
    }
}
