using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class SmallShadowBall
{
    public void RollingLaser(NPC bigBall)
    {
        const int _0_GatherAndSlowDown = 0;
        const int _1_FireByLayer = 1;
        const int _2_ShootLayser = 2;
        const int _3_Restart = 3;

        const int LaserShootTime = 60;

        ShadowBall ball = (bigBall.ModNPC as ShadowBall);

        int layer = (int)Recorder2;
        ShellMove();

        switch (SonState)
        {
            default:
            case _0_GatherAndSlowDown://根据球层决定前摇
                {
                    if (Timer > (LaserShootTime + ShadowBall.ShadowSpike_SmallBallChannelTime()) * layer)
                    {
                        SonState = _1_FireByLayer;
                        Timer = 0;

                        int time = ShadowBall.ShadowSpike_SmallBallChannelTime();
                        SpawnAimLine(16 * 40, time / 3, time / 3 * 2);
                    }
                }
                break;
            case _1_FireByLayer:
                {
                    if (Timer > ShadowBall.ShadowSpike_SmallBallChannelTime())
                    {
                        Timer = 0;
                        SonState = _2_ShootLayser;
                        NPC.NewProjectileDirectInAI_Server<SmallLaser>(NPC.Center, Vector2.Zero,
                            Helper.ScaleValueForDiffMode(25, 35, 45, 55), 0,
                            ai0: NPC.whoAmI, ai1: LaserShootTime);
                    }
                }
                break;
            case _2_ShootLayser:
                {
                    if (Timer > LaserShootTime)
                    {
                        Recorder4++;
                        if (Recorder4 > 1)
                        {
                            SwitchState(AIStates.Idle);
                            return;
                        }

                        SonState = _3_Restart;
                        Timer = 0;
                    }
                }
                break;
            case _3_Restart:
                {
                    if (Timer > (LaserShootTime + ShadowBall.ShadowSpike_SmallBallChannelTime()) * 2)
                    {
                        SonState = _1_FireByLayer;
                        Timer = 0;
                        int time = ShadowBall.ShadowSpike_SmallBallChannelTime();
                        SpawnAimLine(16 * 40, time / 3, time / 3 * 2);
                    }
                }
                break;
        }

        void ShellMove()
        {
            int shell = (int)Recorder2;
            float index = (int)Recorder;
            int howmany = (int)Recorder3;

            float zyRot = 0;
            float xyRot = 0;
            float baseRot = ball.LockTimer * (0.005f+shell*0.003f) * (shell % 2 == 0 ? 1 : -1) ;

            Vector2 targetPos = _3DRotate(index / howmany, 120 + shell * 30, baseRot, zyRot, xyRot) + bigBall.Center;

            NPC.Center = Vector2.SmoothStep(NPC.Center, targetPos, 0.2f);
            NPC.rotation = NPC.rotation.AngleLerp((NPC.Center - bigBall.Center).ToRotation(), 0.2f);
        }
    }
}
