using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>伽玛射线暴的小球占位状态，由本体下令进入并在收招时恢复待机。</summary>
    [VaultState((int)SmallShadowBallStateId.GammaRayBurst, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallGammaRayBurstState : SmallShadowBallStateBase
    {
        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.GammaRayBurst;

        // TODO：补充小球在伽玛射线暴期间的招式 AI。
    }
}
