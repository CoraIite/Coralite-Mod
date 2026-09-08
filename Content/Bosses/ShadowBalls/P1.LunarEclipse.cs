namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void LunarEclipse()
    {
        const int CallSmallBalls = 0;
        const int WaitForEclipse = 1;

        switch (SonState)
        {
            default:
            case CallSmallBalls:
                {
                    if (!BeginSmallBallAttack(SmallShadowBall.AIStates.LunarEclipse))
                        return;

                    SonState = WaitForEclipse;
                    Timer = 0;
                }
                break;
            case WaitForEclipse:
                {
                    NPC.velocity *= 0.95f;
                    if ((Timer > 400 && CheckSmallBallReady()) || Timer > 460)
                        SwitchP1State();
                }
                break;
        }
    }
}
