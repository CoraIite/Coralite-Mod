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
                        Recorder = zDepth;
                        Recorder2 = NPC.Center.X;
                        Recorder3 = NPC.Center.Y;

                        SonState = 1;
                        Timer = 0;
                    }
                    break;
                case 1://固定时间的渐变过去
                    {
                        int lerpTime = ShadowBall.ShadowSpike_SmallBallLerpTime();

                        float f = Helper.BezierEase(Timer / lerpTime);

                        int dir = MathF.Sign(Main.player[owner.target].Center.X - owner.Center.X);

                        Vector2 targetPos = owner.Center + new Vector2(dir * ShadowBall.ShadowSpike_PerLength * selfIndex, 0);

                        NPC.velocity = Vector2.Zero;
                        NPC.Center = Vector2.Lerp(new Vector2(Recorder2, Recorder3), targetPos, f);
                        zDepth = Helper.Lerp(Recorder, 1, f);

                        if (Timer > lerpTime)
                        {
                            SonState = 2;
                            Timer = 0;

                            if ((Main.masterMode || Main.getGoodWorld) && !VaultUtils.isServer)//大师模式专属，到达位置后稍微动一下
                            {
                                if (Main.rand.NextBool())//一半动
                                {
                                    NPC.velocity.X = Main.rand.NextFromList(-1, 1) * 4;
                                }
                            }
                        }
                    }
                    break;
                case 2://预备动作，生成线条
                    {
                        NPC.velocity *= 0.9f;
                        NPC.rotation = NPC.rotation.AngleLerp(-MathHelper.PiOver2, 0.1f);

                        if (Timer > ShadowBall.ShadowSpike_SmallBallChannelTime())
                        {
                            SonState = 3;
                            Timer = 0;
                            int damage = Helper.ScaleValueForDiffMode(30, 50, 40, 40);

                            NPC.NewProjectileDirectInAI_Server<SmallLaser>(NPC.Center, Vector2.Zero, damage, 2, ai0: NPC.whoAmI, ai1: ShadowBall.ShadowSpike_SmallBallLaserTime);
                            Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, NPC.Center);
                        }
                    }
                    break;
                case 3://激光发射中
                    {
                        if (Timer > ShadowBall.ShadowSpike_SmallBallLaserTime + 30)
                        {
                            SwitchState(AIStates.Idle);
                        }
                    }
                    break;
            }

        }
    }
}
