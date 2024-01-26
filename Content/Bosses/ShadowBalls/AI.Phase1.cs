using Coralite.Content.WorldGeneration;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class ShadowBall
    {
        #region RollingLaser转圈圈激光
        public void RollingLaser()
        {
            switch (SonState)
            {
                default:
                case 0://让所有小球聚集到指定位置
                    {
                        NPC.TargetClosest();
                        foreach (var ball in smallBalls)
                            (ball.ModNPC as SmallShadowBall).ResetState(SmallShadowBall.AIStates.RollingLaser);

                        SonState++;
                        Timer = 0;
                    }
                    break;
                case 1://检测小球球状态，如果全部准备好了那么就进入下一个阶段
                    {
                        //自身的运动
                        Vector2 targetPos = (CoraliteWorld.shadowBallsFightArea.Center.ToVector2() + Target.Center) / 2;
                        SetDirection(targetPos, out float xLength, out float yLength);

                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, xLength, NPC.direction
                            , 3f, 32, 0.08f, 0.1f, 0.97f);
                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, yLength, NPC.direction
                            , 3f, 16, 0.08f, 0.1f, 0.97f);

                        NPC.rotation += 0.05f;

                        if (CheckSmallBallsReady())//全准备好了
                        {
                            foreach (var ball in smallBalls)
                                (ball.ModNPC as SmallShadowBall).RollingLaser_OnAllReady(NPC);

                            SonState++;
                        }
                    }
                    break;
                case 2://此时是小球转转射激光，就稍等一会随便动一动等待小球完成射击
                    {
                        //自身的运动
                        NPC.velocity *= 0.96f;

                        if (CheckSmallBallsReady())
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 3://重设状态，或许会在这里稍等一会
                    {
                        if (Timer>30)
                        {
                            ResetState();
                        }
                    }
                    break;
            }
        }
        #endregion

        #region ConvergeLaser汇集后射激光
        public void ConvergeLaser()
        {
            switch (SonState)
            {
                default:
                case 0://让所有小球都汇聚到自己身前
                    {
                        NPC.TargetClosest();
                        foreach (var ball in smallBalls)
                            (ball.ModNPC as SmallShadowBall).ResetState(SmallShadowBall.AIStates.ConvergeLaser);

                        SonState++;
                        Timer = 0;
                    }
                    break;
                    case 1://检测小球状态，如果好了那么进入下一个阶段
                    {
                        Vector2 targetPos = (CoraliteWorld.shadowBallsFightArea.Center.ToVector2() + Target.Center) / 2;
                        SetDirection(targetPos, out float xLength, out float yLength);

                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, xLength, NPC.direction
                            , 3f, 32, 0.08f, 0.1f, 0.97f);
                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, yLength, NPC.direction
                            , 3f, 16, 0.08f, 0.1f, 0.97f);

                        NPC.rotation += 0.05f;

                        if (CheckSmallBallsReady())//全准备好了
                        {
                            foreach (var ball in smallBalls)
                                (ball.ModNPC as SmallShadowBall).RollingLaser_OnAllReady(NPC);

                            SonState++;
                        }
                    }
                    break;
                case 2://此时是小球蓄力射激光，就稍等一会随便动一动等待小球完成射击
                    {
                        //自身的运动
                        NPC.velocity *= 0.96f;
                        NPC.rotation += 0.1f;
                        if (CheckSmallBallsReady())
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 3://重设状态，或许会在这里稍等一会
                    {
                        if (Timer > 30)
                        {
                            ResetState();
                        }
                    }
                    break;

            }
        }

        #endregion
    }
}
