using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void RollingLaser()
    {
        const int CallSmallBalls = 0;
        const int KeepDistance = 1;

        switch (SonState)
        {
            default:
            case CallSmallBalls:
                {
                    if (!BeginSmallBallAttack(SmallShadowBall.AIStates.RollingLaser))
                        return;

                    /*
                     * 设置所有小球状态
                     * 小于3个就重设状态
                     */

                    int smallBallCount = smallBalls.Count;
                    if (smallBallCount<3)
                    {
                        SwitchP1State();
                        return;
                    }

                    //将小球分为3组
                    int perSetSmallBallCount = smallBallCount / 3;

                    for (int i = 0; i < 3; i++)//
                    {
                        for (int j = 0; j < perSetSmallBallCount; j++)
                        {
                            SmallShadowBall smallShadowBall = smallBalls[j + i * perSetSmallBallCount].ModNPC as SmallShadowBall;
                            smallShadowBall.Recorder = j;//recorder用于每层自身的索引
                            smallShadowBall.Recorder2 = i;//recorder2用于球层
                            smallShadowBall.Recorder3 = perSetSmallBallCount;//recorder3记录每层多少小球
                        }
                    }

                    SonState = KeepDistance;
                    Timer = 0;
                }
                break;
            case KeepDistance:
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity,
                        (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 2, 0.03f);

                    if ((Timer > 300 && CheckSmallBallReady()) || Timer > 350)
                        SwitchP1State();
                }
                break;
        }
    }
}
