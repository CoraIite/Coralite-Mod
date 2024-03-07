using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void DashDischarging()
        {
            const int bigDashTime = 60;
            const int burstTime = 35;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://先短暂远离准备冲刺
                    {
                        if (Timer==0)
                        {
                            NPC.frame.Y = 1;
                            Timer = 1;
                        }

                        NPC.QuickSetDirection();
                        SetRotationNormally(0.2f);

                        if (NPC.velocity.Length() < 8)
                            NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.65f;

                        UpdateAllOldCaches();
                        canDrawShadows = true;
                        shadowScale = 1.2f;

                        //向后扇一下翅膀
                        if (++NPC.frameCounter > 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 7)
                            {
                                SonState++;
                                Timer = 0;

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                                DashFrame();

                                NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 40;
                                NPC.rotation = NPC.velocity.ToRotation();
                                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                                isDashing = true;
                                SetBackgroundLight(0.4f, bigDashTime - 3, 8);
                            }
                        }
                    }
                    break;
                case 1://开冲，直到与玩家距离小于一定值后停止
                    {
                        UpdateAllOldCaches();
                        GetLengthToTargetPos(Target.Center, out _, out float yLength);
                        if (yLength > 50)
                            NPC.QuickSetDirection();

                        NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 40;
                        NPC.rotation = NPC.velocity.ToRotation();

                        Timer++;
                        if (Timer > bigDashTime || Vector2.Distance(Target.Center, NPC.Center) < 200)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.frame.X = 0;
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0;
                            isDashing = false;
                        }
                    }
                    break;
                case 2://蓄力！
                    {
                        UpdateAllOldCaches();
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);
                        NPC.QuickSetDirection();
                        TurnToNoRot(0.2f);

                        if (xLength > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 4f, 0.15f, 0.4f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (yLength > 70)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4f, 0.15f, 0.3f, 0.95f);
                        else
                            NPC.velocity.Y *= 0.95f;

                        const int maxTime = 7 * 4 + 20;
                        Timer++;
                        float edge = 200 + 420 * Math.Clamp(Timer / maxTime, 0, 1);
                        edge /= 2;
                        for (int i = 0; i < 4; i++)
                        {
                            SpawnDischargingDust(edge);
                        }

                        if (NPC.frame.Y != 4)
                        {
                            if (++NPC.frameCounter > 7)
                            {
                                NPC.frameCounter = 0;
                                NPC.frame.Y++;
                            }
                        }
                        else
                        {
                            if (Timer > maxTime)
                            {
                                SonState++;
                                Timer = 0;

                                NPC.TargetClosest();
                                int damage = Helper.GetProjDamage(80, 100, 120);
                                NPC.NewProjectileDirectInAI<StrongDischargingBurst>(NPC.Center, Vector2.Zero, damage, 0, NPC.target
                                    , burstTime, NPC.whoAmI);

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, NPC.Center);
                                canDrawShadows = true;
                                currentSurrounding = true;
                                SetBackgroundLight(0.5f, burstTime - 3, 8);

                                ResetAllOldCaches();
                            }
                        }
                    }
                    break;
                case 3://MD跟你爆了！！！！！！！！！！！
                    {
                        UpdateAllOldCaches();
                        TurnToNoRot(0.2f);
                        float factor = Coralite.Instance.SqrtSmoother.Smoother(Timer / burstTime);
                        shadowScale = Helper.Lerp(1f, 2.5f, factor);
                        shadowAlpha = Helper.Lerp(1f, 0f, factor);

                        if (NPC.frame.Y != 0)
                        {
                            NPC.frame.X = 1;

                            if (++NPC.frameCounter > 1)
                            {
                                NPC.frameCounter = 0;
                                if (++NPC.frame.Y > 7)
                                    NPC.frame.Y = 0;
                            }
                        }
                        Timer++;
                        if (Timer > burstTime)
                        {
                            canDrawShadows = false;
                            currentSurrounding = false;

                            Timer = 0;
                            SonState++;
                        }
                    }
                    break;
                case 4://后摇
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 20)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
