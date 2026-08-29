using System;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class ShadowBall
{
    public void OnSpawnAnmi()
    {
        LightStrength = 1;
        MaskAlpha = 0f;

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
        if (Timer == 1)
        {
            //SkyManager.Instance.Activate(nameof(StarlinesSky));

            if (!SkyManager.Instance[nameof(StarlinesSky)].IsActive())//如果这个天空没激活
                SkyManager.Instance.Activate(nameof(StarlinesSky));
        }

        if (Timer > 240)
        {
            if (smallBalls.Count == 0)
            {
                SwitchP1State();
            }
            else
            {
                SwitchState_Test(AIStates.ShadowSpike);
                Recorder = 5;

                //switch (LockState)
                //{
                //    case LockStates.Normal:
                //        //SwitchLockState(LockStates.ConcentricCircles);
                //        break;
                //    case LockStates.ConcentricCircles:
                //        SwitchLockState(LockStates.ConcentricCirclesAngled);
                //        break;
                //    case LockStates.ConcentricCirclesAngled:
                //        SwitchLockState(LockStates.AngledRotate);
                //        break;
                //    case LockStates.AngledRotate:
                //        SwitchLockState(LockStates.Normal);
                //        break;
                //    default:
                //        break;
                //}
                //Timer = 0;
            }
        }

        switch (SonState)
        {
            default:
            case 0://探出脑袋
                {
                    if (Timer < MaxFrameY * 2)
                    {
                        LayerAlpha = Timer / (MaxFrameY * 2);

                        if (Timer % 2 == 0)
                        {
                            if (ShellFrame > 0)
                                ShellFrame--;
                        }
                    }
                    else
                        SonState = 1;
                }
                break;
            case 1:
                {
                    ShellFrame = 30;
                    //ShellFrame = 17 - (int)(MathF.Cos(Timer / 240f * MathHelper.TwoPi) * 17);
                }
                break;
        }
    }
}
