using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void CrownStrike()
        {
            switch ((int)SonState)
            {
                case 0: //缩小的阶段
                    if (Timer < 2)
                        NPC.noGravity = true;

                    OnChangeToCrownMode();
                    ScaleToTarget(1.1f, 0.7f, 0.1f, Scale.Y < 0.75f || Timer > 20, () =>
                    {
                        SonState = 1;
                        Timer = 0;
                    });
                    break;
                case 1:
                    OnChangeToCrownMode();
                    ScaleToTarget(0.6f, 0.8f, 0.1f, Scale.X < 0.65f || Timer > 20, () =>
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                        SonState = 2;
                        Timer = 0;
                    });
                    break;
                case 2:
                    OnChangeToCrownMode();
                    ScaleToTarget(0.7f, 0.5f, 0.1f, Scale.Y < 0.55f || Timer > 20, () =>
                    {
                        SonState = 3;
                        Timer = 0;
                    });
                    break;
                case 3:
                    OnChangeToCrownMode();
                    ScaleToTarget(0.4f, 0.6f, 0.1f, Scale.X < 0.45f || Timer > 20, () =>
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                        SonState = 4;
                        Timer = 0;
                        CrownMode();
                        NPC.TargetClosest();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int howMany = Helper.ScaleValueForDiffMode(1, 2, 4, 6);
                            for (int i = 0; i < howMany; i++)
                            {
                                Point pos = NPC.Center.ToPoint();
                                pos.X += Main.rand.Next(-NPC.width, NPC.width);
                                pos.Y += Main.rand.Next(-32, 32);
                                NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos.X, pos.Y, ModContent.NPCType<ElasticGelBall>());
                                npc.velocity = Helper.NextVec2Dir(2, 5);
                            }
                        }
                    });
                    break;
                case 4: //变成王冠之后简单追踪玩家的阶段
                    {
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
                    }
                    break;
                case 5://向玩家冲撞
                    {
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

                        //FTW中生成弹力球(暂未实装)

                        for (int i = 0; i < 2; i++)
                            Helper.SpawnTrailDust(NPC.Center + Main.rand.NextVector2Circular(80, 80), DustID.TintableDust, (d) => -NPC.velocity * Main.rand.NextFloat(0.2f, 0.4f),
                                150, new Color(78, 136, 255, 80), 2f);

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
                    }
                    break;
                case 6: //变回来
                    OnChangeToCrownMode();
                    ScaleToTarget(1f, 1f, 0.15f, Scale.X > 0.97f, () =>
                    {
                        if (Main.masterMode)
                        {
                            int howMany = Helpers.Helper.ScaleValueForDiffMode(2, 2, 3, 4);
                            int damage = Helper.GetProjDamage(30, 45, 55);

                            for (int i = 0; i < howMany; i++)
                            {
                                Vector2 vel = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 10;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 3, NPC.height / 3), vel, ModContent.ProjectileType<SpikeGelBall>(),
                                    damage, 4f, NPC.target);
                            }
                        }
                        Scale = Vector2.One;
                        SonState = 7;
                        Timer = 0;
                    });
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

            SpawnSplitGelDust();
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
