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
                    if (smallBallCount < 3)
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
                            smallShadowBall.SwitchState(SmallShadowBall.AIStates.RollingLaser);
                            smallShadowBall.Recorder = j;//recorder用于每层自身的索引
                            smallShadowBall.Recorder2 = i;//recorder2用于球层
                            smallShadowBall.Recorder3 = perSetSmallBallCount;//recorder3记录每层多少小球
                        }
                    }

                    for (int i = 0; i < smallBalls.Count; i++)
                    {
                        if (i >= perSetSmallBallCount * 3)
                        {
                            SmallShadowBall smallShadowBall = smallBalls[i].ModNPC as SmallShadowBall;
                            smallShadowBall.SetReady();
                        }
                    }

                    SwitchLockState(LockStates.ConcentricCircles);
                    SonState = KeepDistance;
                    Timer = 0;
                }
                break;
            case KeepDistance:
                {
                    Vector2 aimPos = Target.Center + new Vector2(0, -300);
                    if (Vector2.DistanceSquared(NPC.Center, aimPos) > (16 * 5) * (16 * 5))
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity,
                            (aimPos - NPC.Center).SafeNormalize(Vector2.Zero) * 4, 0.03f);
                    }
                    else
                        NPC.velocity *= 0.9f;

                    if ((Timer > 800 && CheckSmallBallReady()))
                        SwitchState_Test(AIStates.OnSpawnAnmi);
                }
                break;
        }
    }
}
