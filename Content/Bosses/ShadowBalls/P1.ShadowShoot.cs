namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void ShadowShoot()
    {
        const int CallSmallBalls = 0;
        const int IlluminateShadows = 1;

        switch (SonState)
        {
            default:
            case CallSmallBalls:
                {
                    if (!BeginSmallBallAttack(SmallShadowBall.AIStates.ShadowShoot))
                        return;

                    SonState = IlluminateShadows;
                    Timer = 0;
                }
                break;
            case IlluminateShadows:
                {
                    NPC.velocity *= 0.92f;
                    if ((Timer > 240 && CheckSmallBallReady()) || Timer > 290)
                        SwitchP1State();
                }
                break;
        }
    }
}
