using System;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void OnSpawnAnmi()
    {
        LightStrength = 1;
        MaskAlpha = 0f;

        Timer++;

        //NPC.velocity.X = MathF.Sin(Timer * 0.02f)*10;
        LockDistancePercent = 1;// + MathF.Sin(Timer * 0.05f)*0.4f;
        /*
         * 从裂隙中探出发光核心
         * 之后裂隙生成一堆影子弹幕
         * 影子弹幕聚集到核心身上逐渐出现求成，之后开始吼叫？
         * 吼叫时候生成环与名字
         * 
         * 之后切换到召唤小影子球阶段
         */

        if (Timer>40)
        {
            SwitchState_Test(AIStates.SummonSmallShdowBall);
        }

        switch (SonState)
        {
            default:
            case 0://探出脑袋
                break;
        }
    }
}
