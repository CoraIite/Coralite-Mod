using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class ShadowBall
    {
        public const float ShadowSpike_PerLength=16*7;

        public void ShadowSpike()
        {
            /*
             * 影刺
             * 
             * 1. 落到玩家下方
             * 
             * 2.让小球到目标点
             * 
             * 3.发出光并让小球生成预判线，大师模式中小球还会额外移动
             * 
             * 4.小球发射光束期间
             * 
             * 5.结束，收回小球
             * 
             */

            switch (SonState)
            {
                default:
                case 0://确定移动位置
                    {
                        NPC.velocity *= 0.95f;

                        if (Timer > 10)//短暂前摇，找到玩家下方，同时X方向略微预判玩家
                        {
                            Vector2 aimPos = Target.Center + new Vector2(0, 350);
                            float distance = Vector2.Distance(NPC.Center, Target.Center);
                            if (Target.velocity.LengthSquared() > 1)//玩家有速度，直接叠加预判位置
                            {
                                aimPos.X += (Target.velocity.SafeNormalize(Vector2.Zero)
                                    * Helper.Clamp(distance / 600f, 0.2f, 1f) * 300).X;
                            }

                            aimPos.X += ((Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * Helper.Clamp(distance / 800f, 0.2f, 1f)).X * 300;

                            GravityMoveMentReady(aimPos);

                            SonState = 1;
                            Timer = 0;
                        }
                    }
                    break;
                case 1:
                    {
                        if (GravityMovement())
                        {
                            SwitchLockState(LockStates.Normal);
                            SonState = 2;
                            Timer = 0;

                            //呼叫小球

                            int smallBallCount = GetSmallBalls();

                            if (smallBallCount < 1)//怎么个事呢咋一个都没有
                                return;

                            //检测与玩家的X距离
                            float length = MathF.Abs(Target.Center.X - NPC.Center.X);

                            int howMany = (int)(length / ShadowSpike_PerLength) + 6;
                            if (howMany > smallBallCount)
                                howMany = smallBallCount;

                            for (int i = 0; i < howMany; i++)
                            {
                                (smallBalls[i].ModNPC as SmallShadowBall).SwitchState(SmallShadowBall.AIStates.ShadowSpike);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 影刺阶段的小球移动时间
        /// </summary>
        /// <returns></returns>
        public static int ShadowSpike_SmallBallLerpTime()
            => Helper.ScaleValueForDiffMode(45, 40, 35, 20);
    }
}
