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
                        foreach (var ball in smallBalls)
                            (ball.ModNPC as SmallShadowBall).ResetState(SmallShadowBall.AIStates.RollingLaser);

                        SonState++;
                    }
                    break;
                case 1://检测小球球状态，如果全部准备好了那么就进入下一个阶段
                    {
                        //自身的运动

                        NPC.rotation += 0.1f;

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
                        foreach (var ball in smallBalls)
                            (ball.ModNPC as SmallShadowBall).RollingLaser_OnAllReady(NPC);
                    }
                    break;
                case 3://重设状态，或许会在这里稍等一会
                    {

                    }
                    break;
            }
        }
        #endregion
    }
}
