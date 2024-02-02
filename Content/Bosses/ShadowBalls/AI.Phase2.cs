using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class ShadowBall
    {
        #region SmashDown 向下冲刺->升龙/回旋斩

        public void SmashDown()
        {
            switch (SonState)
            {
                default:
                case 0://跳起到指定位置
                    {
                        if (Timer == 2)
                        {
                            NPC.velocity *= 0;
                            //生成跳起的粒子
                        }

                        if (Recorder2 == 0)//自身高度还没达到指定高度时
                        {
                            Vector2 targetPos = Target.Center + new Vector2(0, -400);
                            SetDirection(targetPos, out float xLength, out _);

                            if (xLength > 450)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else if (xLength < 200)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else
                                NPC.velocity.X *= 0.92f;

                            if (NPC.Center.Y < targetPos.Y)//没达到指定高度就向上加速，否则向下
                            {
                                if (NPC.velocity.Y>-16)//向上加速度逐渐递减
                                {
                                    float factor = MathHelper.Clamp(1 - Timer / 25, 0, 1);
                                    NPC.velocity.Y -= 0.08f + factor * 0.1f;
                                }
                            }
                            else
                            {
                                Recorder2 = 1;
                            }
                        }
                        else//自然下落至指定高度
                        {
                            Vector2 targetPos = Target.Center + new Vector2(0, -380);

                            NPC.velocity.X *= 0.95f;
                            if (NPC.velocity.Y<16)
                            {
                                NPC.velocity.Y+=0.2f;
                            }

                            if (NPC.Center.Y >= targetPos.Y)//到达指定高度,准备斩击
                            {
                                SonState++;
                                Timer = 0;
                                NPC.velocity *= 0;
                            }
                        }
                    }
                    break;
                    case 1://向下冲刺到指定位置
                    {
                        const int ReadyTime = 20;
                        const int DashTime = ReadyTime + 8;

                        if (Timer<ReadyTime)//生成闪光
                        {
                            if (Timer==2)
                            {
                                Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(),
                                    Color.Purple, 0.75f);
                            }

                            if (Timer<10)
                            {
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                NPC.rotation = MathHelper.Clamp(NPC.rotation, 3.141f, 0);
                            }
                        }
                        else if(Timer<DashTime)//向下冲刺
                        {
                            NPC.velocity = NPC.rotation.ToRotationVector2() * 40;//冲刺速度
                            if (NPC.Center.Y<Target.Center.Y+80)//低于玩家后停止并在脚下生成一个弹幕平台
                            {

                            }
                        }
                        else
                        {
                        }
                    }
                    break;
                    case 2://进行准备后向上升龙，角度根据玩家位置进行微微调整
                    {

                    }
                    break;
                    case 3://悬停后朝玩家进行回旋斩
                    {

                    }
                    break;
                    case 4://后摇阶段
                    {

                    }
                    break;

            }
        }

        public void OnSmashDown()
        {
            NPC.velocity *= 0;
            SonState++;
            Timer = 0;
            Recorder = 0;
            Recorder2 = 0;

            NPC.NewProjectileInAI<ShadowGround>(NPC.Center + new Vector2(0, NPC.height / 2)
                ,Vector2.Zero,1,0,NPC.target,NPC.whoAmI);

        }

        #endregion

    }
}
