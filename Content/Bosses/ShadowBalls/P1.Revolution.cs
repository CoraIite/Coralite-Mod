using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void Revolution()
    {
        /*
         * 影之公转
         * 
         * 1. 
         * 让背景变暗，自身影子部分变暗，露出内部发光水银核心
         * 
         * 2.将小球召回到身边，形成几个环（根据小球数量决定）
         * 
         * 3.发出光束将小球照出2个不同距离的影子
         * 
         * 4.收回光束
         * 
         */

        const int Ready = 0;
        const int Move = 1;
        const int CallBackSmallBall = 2;
        const int ShootLight = 3;
        const int LightBack = 4;

        switch (SonState)
        {
            default:
            case Ready://确定移动位置
                {
                    if (Timer > 10)//短暂前摇
                    {
                        Vector2 aimPos = Target.Center;
                        float distance = Vector2.Distance(NPC.Center, Target.Center);
                        if (Target.velocity.LengthSquared() > 1)//玩家有速度，直接叠加预判位置
                        {
                            aimPos += Target.velocity.SafeNormalize(Vector2.Zero)
                                * Helper.Clamp(distance / 600f, 0.2f, 1f) * 120;
                        }

                        aimPos += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * Helper.Clamp(distance / 800f, 0.2f, 1f) * 200;


                        GravityMoveMentReady(aimPos);

                        SonState = Move;
                        Timer = 0;
                    }
                }
                break;
            case Move://移动到目标点位
                {
                    if (GravityMovement())
                    {
                        SwitchLockState(LockStates.Normal);
                        SonState = CallBackSmallBall;
                        Timer = 0;

                        //呼叫小球

                        int smallBallCount = GetSmallBalls();

                        if (smallBallCount < 1)//怎么个事呢咋一个都没有
                        {
                            SwitchP1State();
                            return;
                        }

                        //检测与玩家的X距离
                        float length = MathF.Abs(Target.Center.X - NPC.Center.X);
                        const float perLength = 16 * 7;

                        int howMany = (int)(length / perLength) + 6;
                        if (howMany > smallBallCount)
                            howMany = smallBallCount;

                        for (int i = 0; i < howMany; i++)
                        {
                            (smallBalls[i].ModNPC as SmallShadowBall).SwitchState(SmallShadowBall.AIStates.Revolution);
                        }
                    }
                }
                break;
            case CallBackSmallBall://让所有小球运动到准备点
                {
                    if (Timer > 90)
                    {
                        SonState = ShootLight;
                        Timer = 0;
                    }
                }
                break;
            case ShootLight:
                {
                    if (Vector2.DistanceSquared(NPC.Center, Target.Center) > 600 * 600)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 4, 0.04f);
                    else
                        NPC.velocity *= 0.94f;
                    if (!VaultUtils.isClient && Timer % 24 == 0)
                    {
                        int damage = Helper.ScaleValueForDiffMode(20, 30, 40, 50);
                        NPC.NewProjectileDirectInAI_Server<ShadowBallOrbitShadow>(NPC.Center,
                            Main.rand.NextVector2CircularEdge(1, 1) * 6, damage, 0, ai0: 90);
                    }

                    if (Timer > 180)
                    {
                        SonState = LightBack;
                        Timer = 0;
                    }
                }
                break;
            case LightBack:
                {
                    NPC.velocity *= 0.9f;
                    if (Timer > 45)
                        SwitchP1State();
                }
                break;
        }
    }

}
