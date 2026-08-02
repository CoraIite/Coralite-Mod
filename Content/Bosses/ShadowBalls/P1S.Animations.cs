using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class SmallShadowBall
    {
        public void OnSpawnAnmi(NPC bigBall)
        {
            /*
             * 刚生成调整锁扣旋转并让自身向外飞
             * 之后接收到弹幕以后锁扣展开并让自身出现
             * 
             * 自身出现后锁扣旋转一圈切换到idle
             */
            switch (SonState)
            {
                default:
                case 0:
                    {
                        zDepth = 1;
                        LockDistance = 4;
                        lockRotation = (NPC.Center - bigBall.Center).ToRotation();
                        NPC.velocity = (NPC.Center - bigBall.Center).SafeNormalize(Vector2.Zero)*12;

                        SonState = 1;
                        Timer = 0;
                        NPC.scale = 0.0001f;
                    }
                    break;
                case 1:
                    {
                        NPC.velocity *= 0.93f;

                        if (Timer > 60 * 5)//超时，直接进入下一状态
                        {
                            SonState = 2;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://锁扣打开，自身出现
                    {
                        float f = Timer / 30;

                        lockRotation = lockRotation.AngleLerp(-MathHelper.PiOver2, 0.2f);

                        NPC.scale = Helper.SqrtEase(f);
                        LockDistance = Helper.HeavyEase(f) * 40;

                        if (Timer > 30)
                        {
                            SonState = 3;
                            Timer = 0;
                        }
                    }
                    break;
                case 3:
                    {
                        float f = Timer / 25;

                        lockRotation = -MathHelper.PiOver2 + Helper.BezierEase(f) * MathHelper.TwoPi;
                        LockDistance = 40 + Helper.SinEase(f) * 20;

                        if (Timer > 25)//进入Idle
                        {
                            SwitchState(AIStates.Idle);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 接纳汝之暗影
        /// </summary>
        public void AcceptShadow()
        {
            SonState = 2;
            Timer = 0;
        }
    }
}
