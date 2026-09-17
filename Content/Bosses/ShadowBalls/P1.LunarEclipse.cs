using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void LunarEclipse()
    {
        const int _0_StartEffect = 0;
        const int _1_GravityMove = 1;
        const int _2_FacePlayer = 2;
        const int _3_CheckLoopCount = 3;
        const int _4_EndAttack = 4;

        Player targetPlayer = Target;

        switch (SonState)
        {
            default:
            case _0_StartEffect:
                {
                    // TODO: 生成白色圆环特效Particle（先留空）

                    if (Timer >= 45)
                    {
                        // 随机Recorder至0~2Pi
                        Recorder2 = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                        
                        // 目标点为玩家中心加根据Recorder记录角度转换的向量乘以450
                        Vector2 targetPosition = targetPlayer.Center + Recorder.ToRotationVector2() * 450f;
                        GravityMoveMentReady(targetPosition);
                        
                        SonState = _1_GravityMove;
                        Timer = 0;
                    }
                }
                break;
            case _1_GravityMove:
                {
                    // 引力移动
                    if (GravityMovement())
                    {
                        // 使用临时list，遍历小球并将状态为idle的加入这个列表中
                        List<NPC> idleSmallBalls = new List<NPC>();
                        foreach (var smallBall in smallBalls)
                        {
                            if (smallBall != null && smallBall.active)
                            {
                                SmallShadowBall ball = smallBall.ModNPC as SmallShadowBall;
                                if (ball != null && ball.State == (int)SmallShadowBall.AIStates.Idle)
                                {
                                    idleSmallBalls.Add(smallBall);
                                }
                            }
                        }

                        // 从列表中随机选择一个小球，切换小球状态到月食
                        if (idleSmallBalls.Count > 0)
                        {
                            NPC selectedBall = Main.rand.Next(idleSmallBalls);
                            SmallShadowBall ball = selectedBall.ModNPC as SmallShadowBall;
                            if (ball != null)
                            {
                                ball.SwitchState(SmallShadowBall.AIStates.LunarEclipse);
                            }
                        }

                        // 切换锁扣状态为角度跟随大球的旋转的
                        LockState = LockStates.ConcentricCirclesAngled;

                        SonState = _2_FacePlayer;
                        Timer = 0;
                    }
                }
                break;
            case _2_FacePlayer:
                {
                    // 角度持续朝向玩家
                    float f = Helper.Clamp(Timer / 50, 0, 1);

                    NPC.rotation =NPC.rotation.AngleLerp( (targetPlayer.Center - NPC.Center).ToRotation(), f);

                    // 经过100帧后切换到招式状态3
                    if (Timer >= 100)
                    {
                        SonState = _3_CheckLoopCount;
                        Timer = 0;
                    }
                }
                break;
            case _3_CheckLoopCount:
                {
                    // 增加NPC.localAI[2]作为循环计数器
                    Recorder3++;

                    // 判断这个值，小于目标值时切换回招式状态1
                    int targetLoops = Helper.ScaleValueForDiffMode(5, 6, 7, 8); // 普通5/专家6/大师7/FTW8
                    
                    if (Recorder3 < targetLoops)
                    {
                        // 随机增加Recorder，增加1/3Pi~2/3Pi
                        Recorder2 += MathHelper.TwoPi / 3;//Main.rand.NextFloat(MathHelper.Pi / 3f, MathHelper.TwoPi / 3f);
                        
                        // 计算新的目标点
                        Vector2 targetPosition = targetPlayer.Center + Recorder2.ToRotationVector2() * 450f;
                        GravityMoveMentReady(targetPosition);

                        SonState = _1_GravityMove;
                        Timer = 0;
                    }
                    else
                    {
                        // 大于目标值进入招式状态4，切换时遍历小球将小球状态设为idle
                        foreach (var smallBall in smallBalls)
                        {
                            if (smallBall != null && smallBall.active)
                            {
                                if (smallBall.ModNPC is SmallShadowBall ball)
                                {
                                    ball.SwitchState(SmallShadowBall.AIStates.Idle);
                                }
                            }
                        }

                        SonState = _4_EndAttack;
                        Timer = 0;
                    }
                }
                break;
            case _4_EndAttack:
                {
                    // 招式后摇，30帧后切换状态
                    if (Timer >= 30)
                    {
                        SwitchState_Test(AIStates.OnSpawnAnmi);
                        Timer = 0;
                    }
                }
                break;
        }
    }
}
