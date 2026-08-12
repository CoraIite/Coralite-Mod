using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class SmallShadowBall
    {
        public void ShadowSpike(NPC owner)
        {
            /*
             * 先计算自身应该在的位置
             * 然后平移过去
             * 到位之后发射激光
             */

            switch (SonState)
            {
                default:
                case 0://计算该在的位置
                    {
                        Recorder2 = NPC.Center.X;
                        Recorder3 = NPC.Center.Y;

                        SonState = 1;
                        Timer = 0;
                    }
                    break;
                case 1://固定时间的渐变过去
                    {
                        int lerpTime = ShadowBall.ShadowSpike_SmallBallLerpTime();

                        float f =Helper.X3Ease( Timer / lerpTime);

                        int dir = MathF.Sign(Main.player[owner.target].Center.X - owner.Center.X);

                        Vector2 targetPos = owner.Center + new Vector2(dir * ShadowBall.ShadowSpike_PerLength * selfIndex, 0);

                        NPC.velocity = Vector2.Zero;
                        NPC.Center = Vector2.Lerp(new Vector2(Recorder2, Recorder3), targetPos, f);

                        if (Timer>lerpTime)
                        {
                            SonState = 2;
                            Timer = 1;
                        }
                    }
                    break;
            }

        }
    }
}
