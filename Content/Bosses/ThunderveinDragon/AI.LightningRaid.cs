using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void LightningRaidP1()
        {
            const int smallDashTime = 12;
            const int bigDashTime = 25;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://先飞到离玩家比较近或者计时器到点了
                    {
                        const int chasingTime = 60 * 4;
                        NPC.QuickSetDirection();

                        //追踪玩家
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                        if (xLength < 200)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 18f, 0.3f, 0.6f, 0.95f);
                        else if (xLength > 400)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 18f, 0.3f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.35f, 15, 0.9f);
                        else if (yLength > 70)
                        {
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 15f, 0.25f, 0.6f, 0.95f);
                            FlyingFrame();
                        }
                        else
                        {
                            NPC.velocity.Y *= 0.95f;
                            FlyingFrame();
                        }

                        SetRotationNormally();

                        Timer++;
                        if (Timer > chasingTime || (xLength > 200 && xLength < 400 && yLength < 150))
                        {
                            SonState++;
                            Timer = 0;
                            DashFrame();
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            isDashing = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;
                        }
                    }
                    break;
                case 1://开始乱窜几下
                    {
                        if (Timer == 0)
                        {
                            //生成弹幕并随机速度方向
                            NPC.TargetClosest();
                            int damage = Helper.ScaleValueForDiffMode(30, 40, 35, 30);
                            NPC.NewProjectileDirectInAI<LightingDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI,55);
                            
                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();
                           
                            targetrot += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(0.9f, 1f);
                            NPC.velocity = targetrot.ToRotationVector2() * 35;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }
                        else if (Timer % smallDashTime == 0)
                        {
                            NPC.TargetClosest();

                            int damage = Helper.ScaleValueForDiffMode(30, 40, 35, 30);
                            NPC.NewProjectileDirectInAI<LightingDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI,55);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                            
                            float targetrot = (Target.Center - NPC.Center).ToRotation();
                            targetrot += (Timer / smallDashTime > 1 ? -1 : 1) * Main.rand.NextFloat(0.4f, 1f);
                            NPC.velocity = targetrot.ToRotationVector2() * 30;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }

                        UpdateAllOldCaches();

                        Timer++;
                        if (Timer > smallDashTime * 3 - 2)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.frame.X = 0;
                            NPC.frame.Y = 4;
                            NPC.frameCounter = 0;
                            NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                            NPC.TargetClosest();
                            isDashing = false;
                        }
                    }
                    break;
                case 2://准备冲刺！
                    {
                        NPC.QuickSetDirection();

                        SetRotationNormally();

                        if (NPC.velocity.Length() < 8)
                            NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.35f;

                        UpdateAllOldCaches();

                        //向后扇一下翅膀
                        if (++NPC.frameCounter > 7)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 7)
                            {
                                SonState++;
                                Timer = 0;

                                int damage = Helper.ScaleValueForDiffMode(30, 40, 35, 30);
                                NPC.NewProjectileDirectInAI<LightingDash>(NPC.Center, Vector2.Zero, damage, 0
                                    , NPC.target, bigDashTime, NPC.whoAmI,75);

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                                DashFrame();

                                NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 40;
                                NPC.rotation = NPC.velocity.ToRotation();
                                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                                isDashing = true;
                            }
                        }
                    }
                    break;
                case 3://冲刺！冲刺！
                    {
                        UpdateAllOldCaches();

                        if (Timer == bigDashTime)
                        {
                            isDashing = false;
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                            NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                        }
                        else if (Timer > bigDashTime)
                        {
                            NPC.velocity *= 0.8f;
                            FlyingFrame();
                            if (Math.Abs(NPC.velocity.X) < 2f)
                            {
                                NPC.QuickSetDirection();
                                NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                            }

                        }
                        Timer++;
                        if (Timer > bigDashTime + 20)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
