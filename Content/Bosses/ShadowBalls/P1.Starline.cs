using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void Starline()
    {
        const int MoveAbovePlayer = 0;
        const int ReleaseStars = 1;

        switch (SonState)
        {
            default:
            case MoveAbovePlayer:
                {
                    Vector2 targetPosition = Target.Center + new Vector2(0, -360);
                    NPC.velocity = Vector2.Lerp(NPC.velocity,
                        (targetPosition - NPC.Center).SafeNormalize(Vector2.Zero) * 12, 0.08f);

                    if (Timer > 45)
                    {
                        if (!BeginSmallBallAttack(SmallShadowBall.AIStates.Starline))
                            return;

                        SonState = ReleaseStars;
                        Timer = 0;
                    }
                }
                break;
            case ReleaseStars:
                {
                    NPC.velocity *= 0.94f;
                    if (!VaultUtils.isClient && Timer < 72 && Timer % 6 == 0)
                    {
                        float angle = Timer / 6 * 2.17f;
                        int damage = Helper.ScaleValueForDiffMode(18, 25, 32, 40);
                        NPC.NewProjectileDirectInAI_Server<ShadowBallStar>(NPC.Center,
                            angle.ToRotationVector2() * 9, damage, 0, ai0: NPC.whoAmI, ai1: angle);
                    }

                    if ((Timer > 300 && CheckSmallBallReady()) || Timer > 360)
                        SwitchP1State();
                }
                break;
        }
    }
}
