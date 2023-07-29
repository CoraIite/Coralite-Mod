using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void CrownStrike()
        {
            switch ((int)SonState)
            {
                case 0: //缩小的阶段
                    if (Timer <2)
                        NPC.noGravity = true;

                    Scale = Vector2.Lerp(Scale, new Vector2(1.1f,0.7f), 0.1f);
                    OnChangeToCrownMode();

                    if (Scale.Y < 0.75f || Timer > 20)
                    {
                        SonState = 1;
                        Timer = 0;
                    }
                    break;
                case 1:
                    {
                        Scale = Vector2.Lerp(Scale, new Vector2(0.6f, 0.8f), 0.1f);
                        OnChangeToCrownMode();

                        if (Scale.X < 0.65f || Timer > 20)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                            SonState = 2;
                            Timer = 0;
                        }
                    }
                    break;
                case 2:
                    {
                        Scale = Vector2.Lerp(Scale, new Vector2(0.7f, 0.5f), 0.1f);
                        OnChangeToCrownMode();

                        if (Scale.Y < 0.55f || Timer > 20)
                        {
                            SonState = 3;
                            Timer = 0;
                        }
                    }
                    break;
                case 3:
                    {
                        Scale = Vector2.Lerp(Scale, new Vector2(0.4f, 0.6f), 0.1f);
                        OnChangeToCrownMode();

                        if (Scale.X < 0.45f || Timer > 20)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                            SonState = 4;
                            Timer = 0;
                            CrownMode();
                            NPC.TargetClosest();

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int howMany = Helper.ScaleValueForDiffMode(1, 2, 2, 3);
                                for (int i = 0; i < howMany; i++)
                                {
                                    Point pos = NPC.Center.ToPoint();
                                    pos.X += Main.rand.Next(-NPC.width, NPC.width);
                                    pos.Y += Main.rand.Next(-32, 32);
                                    NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos.X, pos.Y, ModContent.NPCType<ElasticGelBall>());
                                    npc.velocity = Helper.NextVec2Dir() * Main.rand.NextFloat(2, 5);
                                }
                            }

                        }
                    }
                    break;
                case 4: //变成王冠之后简单追踪玩家的阶段
                    //和玩家保持一定距离
                    if (Timer < 100)
                    {
                        Vector2 targetVec = Target.Center - NPC.Center;
                        Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);
                        float length = targetVec.Length();
                        if (length < 400)
                            NPC.velocity -= dir * 0.5f;
                        else
                            NPC.velocity += dir * 0.5f;

                        if (NPC.velocity.Length() > 8)
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 8;

                        break;
                    }

                    SonState = 5;
                    Timer = 0;
                    NPC.TargetClosest();

                    break;
                case 5://向玩家冲撞
                    if (Timer < 30)
                    {
                        NPC.velocity *= 0.96f;
                        break;
                    }

                    //生成冲刺时的粒子
                    if (Timer < 32)
                    {
                        Vector2 targetVec = Target.Center - NPC.Center;
                        Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);

                        SoundEngine.PlaySound(CoraliteSoundID.SlimeMount_Item81, NPC.Center);
                        for (int i = 0; i < 12; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(80, 80), DustID.TintableDust,
                                -dir * Main.rand.NextFloat(0.2f, 4f), 150, new Color(78, 136, 255, 80), 2f);
                            dust.noGravity = true;
                        }
                        NPC.velocity = dir * 10f;
                        break;
                    }

                    //FTW中生成弹力球

                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(80, 80), DustID.TintableDust,
                            -NPC.velocity * Main.rand.NextFloat(0.2f, 0.4f), 150, new Color(78, 136, 255, 80), 2f);
                        dust.noGravity = true;
                    }

                    if (Timer > 105)
                    {
                        NPC.velocity *= 0.96f;
                        if (Timer > 115)
                        {
                            NPC.velocity.X *= 0;
                            SonState = 6;
                            Timer = 0;
                            SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                            SlimeMode();
                        }
                    }
                    break;
                case 6: //变回来
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.15f);
                    OnChangeToCrownMode();

                    if (Scale.X > 0.97f)
                    {
                        Scale = Vector2.One;
                        SonState = 7;
                        Timer = 0;
                    }
                    break;
                default:
                case 7:
                    ResetStates();
                    break;
            }

            Timer++;
        }

        /// <summary>
        /// 在变为王冠模式的过程中使用到，包含减速，固定王冠位置，改变帧图
        /// </summary>
        private void OnChangeToCrownMode()
        {
            NPC.velocity *= 0.96f;

            int height = GetCrownBottom();
            float groundHeight = NPC.Bottom.Y - Scale.Y * height;

            crown.Bottom.Y = groundHeight;

            int width = (int)(NPC.width * Scale.X * 0.7f);
            int height2 = (int)(NPC.height * Scale.Y * 0.7f);
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(width, height2), DustID.TintableDust,
                    Helpers.Helper.NextVec2Dir() * Main.rand.NextFloat(0.2f, 1f), 150, new Color(78, 136, 255, 80), 2f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }

            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y++;
                if (NPC.frame.Y > 3)
                    NPC.frame.Y = 0;
            }
        }
    }
}
