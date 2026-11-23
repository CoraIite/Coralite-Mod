using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.MucuryBroken, typeof(ShadowBallContext))]
    public class MucuryBroken : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.MucuryBroken;

        private enum Beat
        {
            Ready,
            Shoot,
            SlowDown,
            Over,
        }


        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    {
                    }
                    break;
                case Beat.Shoot:
                    break;
                case Beat.SlowDown:
                    {
                    }
                    break;
                case Beat.Over:
                    break;
                default:
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    break;
                case Beat.Shoot:
                    break;
                case Beat.SlowDown:
                    break;
                case Beat.Over:
                    return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.MucuryBurst);
                default:
                    break;
            }

            return null;
        }
    }
}
