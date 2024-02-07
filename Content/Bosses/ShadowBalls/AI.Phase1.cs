using Coralite.Content.WorldGeneration;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class ShadowBall
    {
        #region 生成动画

        /*
         * 之后在中心出现一下闪光，展开圆形弹幕         
         * 点击灯之影后出现一个弹幕落下到场地中心的位置
         * 之后影子球自下而上地出现，并生成吼叫粒子
         * 之后召唤小球
         */

        public void OnSpawnAnmi()
        {
            switch (SonState)
            {
                default:
                case 0://生成一些粒子
                    {
                        if (Timer == 2)
                        {
                            NPC.TargetClosest();
                            if (!SkyManager.Instance["StarsBackSky"].IsActive())//如果这个天空没激活
                                SkyManager.Instance.Activate("StarsBackSky");
                        }
                        const int SpawnDustTime = 90;

                        if (Timer > SpawnDustTime)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://自下而上地出现
                    {
                        const int ShowUpTime = 60;

                        SpawnOverflowHeight = Timer / ShowUpTime;
                        //生成粒子挡住

                        if (Timer > 60)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://生成吼叫粒子和名称
                    {
                        //NPC.dontTakeDamage = false;
                        InitCaches();
                        ResetState();
                        SpawnSmallBalls();
                    }
                    break;
                case 3://生成小球并等待小球完成生成
                    {

                    }
                    break;

            }
        }

        #endregion

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
                        //自身的运动，会尝试和玩家保持一定距离，并且会将自身限制在一个框里
                        Vector2 targetPos =  Target.Center;
                        SetDirection(targetPos, out float xLength, out float yLength);

                        if (xLength > 450)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                , 5f, 0.1f, 0.18f, 0.97f);
                        else if (xLength < 200)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                , 5f, 0.1f, 0.18f, 0.97f);
                        else
                            NPC.velocity.X *= 0.92f;

                        //控制Y方向的移动
                        if (yLength > 350)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                , 5f, 0.1f, 0.18f, 0.97f);
                        else if (yLength < 200)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, -NPC.directionY
                                , 5f, 0.1f, 0.18f, 0.97f);
                        else
                            NPC.velocity.Y *= 0.92f;

                        NPC.rotation += 0.05f;
                        MovementLimit();

                        if (CheckSmallBallsReady())//全准备好了
                        {
                            foreach (var ball in smallBalls)
                                (ball.ModNPC as SmallShadowBall).RollingLaser_OnAllReady(NPC);

                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://此时是小球转转射激光，就稍等一会随便动一动等待小球完成射击
                    {
                        if (Timer < 60)
                        {
                            //自身的运动
                            Vector2 targetPos = Target.Center;
                            SetDirection(targetPos, out float xLength, out float yLength);

                            if (xLength > 450)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else if (xLength < 200)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else
                                NPC.velocity.X *= 0.92f;

                            //控制Y方向的移动
                            if (yLength > 350)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else if (yLength < 200)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, -NPC.directionY
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.92f;
                        }
                        else
                        {
                            NPC.velocity *= 0.95f;
                        }

                        MovementLimit();

                        if (CheckSmallBallsReady())
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity = Vector2.Zero;
                        }
                    }
                    break;
                case 3://重设状态，或许会在这里稍等一会
                    {
                        //if (Timer>30)
                        //{
                        ResetState();
                        //}
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
                        Vector2 targetPos = Target.Center;
                        SetDirection(targetPos, out float xLength, out float yLength);

                        if (xLength > 450)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                , 3f, 0.1f, 0.18f, 0.97f);
                        else if (xLength < 200)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                , 3f, 0.1f, 0.18f, 0.97f);
                        else
                            NPC.velocity.X *= 0.92f;

                        //控制Y方向的移动
                        if (yLength > 350)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                , 3f, 0.1f, 0.18f, 0.97f);
                        else if (yLength < 200)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, -NPC.directionY
                                , 3f, 0.1f, 0.18f, 0.97f);
                        else
                            NPC.velocity.Y *= 0.92f;

                        MovementLimit();

                        NPC.rotation += 0.05f;

                        if (CheckSmallBallsReady())//全准备好了
                        {
                            foreach (var ball in smallBalls)
                                (ball.ModNPC as SmallShadowBall).ConvergeLaser_OnAllReady(NPC);

                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://此时是小球蓄力射激光，就稍等一会随便动一动等待小球完成射击
                    {
                        if (Timer < 50)
                        {
                            //自身的运动
                            Vector2 targetPos = Target.Center;
                            SetDirection(targetPos, out float xLength, out float yLength);

                            if (xLength > 450)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , 3f, 0.1f, 0.18f, 0.97f);
                            else if (xLength < 200)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                    , 3f, 0.1f, 0.18f, 0.97f);
                            else
                                NPC.velocity.X *= 0.92f;

                            //控制Y方向的移动
                            if (yLength > 350)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                    , 3f, 0.1f, 0.18f, 0.97f);
                            else if (yLength < 200)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, -NPC.directionY
                                    , 3f, 0.1f, 0.18f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.92f;

                            MovementLimit();
                        }
                        else
                        {
                            NPC.velocity *= 0.9f;
                        }

                        NPC.rotation += 0.1f;
                        if (CheckSmallBallsReady())
                        {
                            SonState++;
                            Timer = 0;
                            foreach (var ball in smallBalls)
                                (ball.ModNPC as SmallShadowBall).Sign = (int)SmallShadowBall.SignType.Nothing;
                        }
                    }
                    break;
                case 3://此时小球全部就位，停止自身旋转
                    {
                        NPC.velocity *= 0.96f;
                        if (CheckSmallBallsReady())
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 4://重设状态，或许会在这里稍等一会
                    {
                        //if (Timer > 30)
                        //{
                        ResetState();
                        //}
                    }
                    break;

            }
        }

        #endregion

        #region LaserWithBeam激光+光束

        public void LaserWithBeam()
        {
            switch (SonState)
            {
                default:
                case 0://让一个小球进入射激光状态，其他变成射光束状态
                    {
                        //随机一下持续时间
                        Timer = Main.rand.Next(60 * 8, 60 * 12);

                        int whoToShootLaser = Main.rand.Next(0, smallBalls.Count);
                        for (int i = 0; i < smallBalls.Count; i++)
                        {
                            if (i == whoToShootLaser)
                                (smallBalls[i].ModNPC as SmallShadowBall)
                                    .Idle(SmallShadowBall.AIStates.LaserWithBeam_Laser, 30);
                            else
                                (smallBalls[i].ModNPC as SmallShadowBall)
                                    .Idle(SmallShadowBall.AIStates.LaserWithBeam_Beam, Main.rand.Next(2, 60));
                        }

                        SonState++;
                    }
                    break;
                case 1://随便动一动等待计时结束
                    {
                        //自身运动

                        if (Timer <= 0)
                        {
                            foreach (var smallBall in smallBalls)
                            {
                                if (smallBall.ai[1] == (int)SmallShadowBall.AIStates.LaserWithBeam_Laser)
                                {
                                    if (smallBall.ai[2] == 3)
                                    {
                                        SonState++;
                                        Timer = 0;
                                    }
                                    break;
                                }
                            }
                        }
                        Timer--;
                    }
                    break;
                case 2://后摇
                    {
                        ResetState();
                    }
                    break;
            }
        }

        #endregion

        #region LeftRightLaser左右激光

        public void LeftRightLaser()
        {
            switch (SonState)
            {
                default:
                case 0://随机小球的状态
                    {
                        //随机一下持续时间
                        Timer = Main.rand.Next(60 * 4, 60 * 7);
                        NPC.TargetClosest();
                        foreach (var ball in smallBalls)
                            (ball.ModNPC as SmallShadowBall).ResetState(SmallShadowBall.AIStates.LeftRightLaser);

                        SonState++;
                    }
                    break;
                case 1://随便动一动等待计时结束
                    {
                        //自身运动
                        Vector2 targetPos = CoraliteWorld.shadowBallsFightArea.Center.ToVector2();
                        SetDirection(targetPos, out float xLength, out float yLength);

                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, xLength, NPC.direction
                            , 3f, 32, 0.08f, 0.1f, 0.97f);
                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.Y, yLength, NPC.directionY
                            , 3f, 16, 0.08f, 0.1f, 0.97f);

                        if (Timer <= 0)
                        {
                            bool allReady = true;
                            foreach (var smallBall in smallBalls)
                            {
                                if (smallBall.ai[2] == 4)//此时表示已准备就绪
                                    continue;

                                if (smallBall.ai[2] == 3)
                                    smallBall.ai[2] = 4;

                                allReady = false;
                            }

                            if (allReady)
                            {
                                SonState++;
                                Timer = 0;
                                break;
                            }
                        }
                        Timer--;
                    }
                    break;
                case 2://后摇
                    {
                        ResetState();
                    }
                    break;
            }
        }

        #endregion

        #region RollingShadowPlayer旋转射弹幕

        public void RollingShadowPlayer()
        {
            switch (SonState)
            {
                default:
                case 0://切换小球状态
                    {
                        NPC.TargetClosest();
                        float value = Main.rand.NextFloat(6.282f);
                        foreach (var ball in smallBalls)
                            (ball.ModNPC as SmallShadowBall).SetRollingShadowPlayer(value);

                        SonState++;
                    }
                    break;
                case 1://等待小球就位，之后进入下一个阶段
                    {
                        //自身的运动
                        Vector2 targetPos = (CoraliteWorld.shadowBallsFightArea.Center.ToVector2() + Target.Center) / 2;
                        SetDirection(targetPos, out float xLength, out float yLength);

                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, xLength, NPC.direction
                            , 3f, 32, 0.08f, 0.1f, 0.97f);
                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.Y, yLength, NPC.directionY
                            , 3f, 16, 0.08f, 0.1f, 0.97f);

                        NPC.rotation += 0.05f;

                        if (CheckSmallBallsReady())//全准备好了
                        {
                            foreach (var ball in smallBalls)
                                (ball.ModNPC as SmallShadowBall).RollingShadowPlayerAllReady();

                            SonState++;
                        }
                    }
                    break;
                case 2://此时小球在设弹幕，等一会
                    {
                        if (CheckSmallBallsReady())//全准备好了
                        {
                            SonState++;
                        }
                    }
                    break;
                case 3://此时小球完成
                    {
                        ResetState();
                    }
                    break;
            }
        }

        #endregion

        #region RandomLaser随机光

        public void RandomLaser()
        {
            switch (SonState)
            {
                default:
                case 0://切换小球状态
                    {
                        NPC.TargetClosest();
                        foreach (var ball in smallBalls)
                            (ball.ModNPC as SmallShadowBall).ResetState(SmallShadowBall.AIStates.RandomLaser);

                        SonState++;
                        Timer = Main.rand.Next(60 * 8, 60 * 12);
                    }
                    break;
                case 1://计时等待小球完成
                    {
                        //自身运动
                        Vector2 targetPos = CoraliteWorld.shadowBallsFightArea.Center.ToVector2();
                        SetDirection(targetPos, out float xLength, out float yLength);

                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, xLength, NPC.direction
                            , 3f, 32, 0.08f, 0.1f, 0.97f);
                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.Y, yLength, NPC.directionY
                            , 3f, 16, 0.08f, 0.1f, 0.97f);

                        if (Timer <= 0)
                        {
                            bool allReady = true;
                            foreach (var smallBall in smallBalls)
                            {
                                if (smallBall.ai[2] == 4)//此时表示已准备就绪
                                    continue;

                                if (smallBall.ai[2] == 3)
                                    smallBall.ai[2] = 4;

                                allReady = false;
                            }

                            if (allReady)
                            {
                                SonState++;
                                Timer = 0;
                                break;
                            }
                        }

                        Timer--;
                    }
                    break;
                case 2://后摇
                    {
                        ResetState();
                    }
                    break;

            }
        }

        #endregion
    }
}
