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
        const int CallBackSmallBall = 1;
        const int ShootLight = 2;
        const int LightBack = 3;

        switch (SonState)
        {
            default:
            case Ready:
                {

                }
                break;
            case CallBackSmallBall:
                {

                }
                break;
            case ShootLight:
                {

                }
                break;
            case LightBack:
                {

                }
                break;
        }
    }

}
