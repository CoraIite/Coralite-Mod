using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void CrownStrike()
        {
            switch ((int)SonState)
            {
                default:
                case 0: //缩小的阶段
                    if (Timer <1)
                    {
                        NPC.noGravity = true;
                        NPC.velocity *= 0;
                    }

                    Scale = Vector2.Lerp(Scale, Vector2.Zero, 0.1f);

                    if (Timer > 20)
                    {
                        SonState = 1;
                        Timer = 0;
                        CrownMode();
                        NPC.TargetClosest();
                    }

                    break;

                case 1: //变成王冠之后简单追踪玩家的阶段
                    //和玩家保持一定距离
                    if (Timer < 100)
                    {
                        Vector2 targetVec = Target.Center - NPC.Center;
                        Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);
                        float length = targetVec.Length();
                        if (length < 280)
                            NPC.velocity -= dir * 0.25f;
                        else
                            NPC.velocity += dir * 0.25f;

                        if (NPC.velocity.Length() > 12)
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 12;

                        break;
                    }

                    SonState = 2;
                    Timer = 0;
                    NPC.TargetClosest();

                    break;
                case 2://向玩家冲撞
                    if (Timer < 20)
                    {
                        NPC.velocity *= 0.98f;
                        break;
                    }

                    //生成冲刺时的粒子
                    if (Timer<22)
                    {
                        Vector2 targetVec = Target.Center - NPC.Center;
                        Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);

                        NPC.velocity = dir * 16f;
                        break;
                    }

                    //FTW中生成弹力球

                    if (Timer > 55)
                    {
                        NPC.velocity *= 0.96f;
                        if (Timer > 65)
                        {
                            SonState = 3;
                            Timer = 0;
                            SlimeMode();
                        }
                    }
                    break;
                case 3: //变回来
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.3f);

                    //生成扩大时的粒子

                    if (Scale.X > 0.97f)
                    {
                        SonState = 4;
                        Timer = 0;
                    }
                    break;
                case 4:
                    ResetStates();
                    break;
            }

            Timer++;
        }
    }
}
