using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        private enum LightningRaidState
        {
            Reset = 0,
            Wait,
            SmallDash,
            Flashing,
            BigDash
        }

        private LightningRaidState[] LightningRaidController { get; set; }

        /// <summary>
        /// 此招式开始前请保证设置<see cref="LightningRaidController"/>的值<br></br>
        /// 使用<see cref="Recorder"/>作为招式进度的记录
        /// </summary>
        /// <returns></returns>
        public bool LightningRaid()
        {
            const int smallDashTime = 12;
            const int bigDashTime = 25;

            switch (RecorderAI)
            {
                default:
                    return true;
                case (int)LightningRaidState.Reset://记录值，重设部分值
                    {
                        //防止意外事故
                        LightningRaidController ??= [LightningRaidState.SmallDash, LightningRaidState.SmallDash, LightningRaidState.SmallDash, LightningRaidState.BigDash];

                        Recorder = 0;

                        return false;
                    }
                //{
                //    const int chasingTime = 60 * 3;
                //    NPC.QuickSetDirection();

                //    //追踪玩家
                //    GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                //    if (xLength < 350)
                //        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 18f, 0.35f, 0.6f, 0.95f);
                //    else if (xLength > 550)
                //        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 18f, 0.35f, 0.6f, 0.95f);
                //    else
                //        NPC.velocity.X *= 0.95f;

                //    if (NPC.directionY < 0)
                //        FlyingUp(0.4f, 15, 0.93f);
                //    else if (yLength > 70)
                //    {
                //        Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 15f, 0.25f, 0.6f, 0.95f);
                //        FlyingFrame();
                //    }
                //    else
                //    {
                //        NPC.velocity.Y *= 0.95f;
                //        FlyingFrame();
                //    }

                //    SetRotationNormally();

                //    Timer++;
                //    if (Timer > chasingTime || (xLength > 350 && yLength < 250 && Timer > 20))
                //    {
                //        SonState++;
                //        Timer = 0;
                //        DashFrame();
                //        ResetAllOldCaches();
                //        canDrawShadows = true;
                //        isDashing = true;
                //        shadowScale = 1.15f;
                //        shadowAlpha = 1;

                //        currentSurrounding = true;
                //    }
                //}
                case (int)LightningRaidState.Wait://扇翅膀向后飞行
                    {

                    }
                    break;
                case (int)LightningRaidState.SmallDash://小冲刺
                    {
                        if (Timer == 0)
                        {
                            //生成弹幕并随机速度方向
                            int damage = Helper.GetProjDamage(20, 30, 70);
                            NPC.NewProjectileDirectInAI<PurpleDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI, 55);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();

                            if (Vector2.Distance(NPC.Center, Target.Center) < 700)
                                targetrot += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(0.9f, 1.1f);
                            NPC.velocity = targetrot.ToRotationVector2() * 35;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }
                        else if (Timer % smallDashTime == 0)
                        {
                            NPC.TargetClosest();

                            int damage = Helper.GetProjDamage(20, 30, 70);
                            NPC.NewProjectileDirectInAI<PurpleDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime, NPC.whoAmI, 55);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);

                            float targetrot = (Target.Center - NPC.Center).ToRotation();
                            if (Vector2.Distance(NPC.Center, Target.Center) < 700)
                                targetrot += (Timer / smallDashTime > 1 ? -1 : 1) * Main.rand.NextFloat(0.6f, 1.1f);
                            NPC.velocity = targetrot.ToRotationVector2() * 30;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                        }

                        //UpdateAllOldCaches();

                        Timer++;
                        if (Timer > (smallDashTime * 3) - 2)
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
                case (int)LightningRaidState.Flashing://使用闪烁的方式移动
                    {
                        NPC.QuickSetDirection();
                        //SetRotationNormally(0.2f);

                        if (NPC.velocity.Length() < 8)
                            NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.65f;

                        //UpdateAllOldCaches();

                        //向后扇一下翅膀
                        if (++NPC.frameCounter > 7)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 7)
                            {
                                SonState++;
                                Timer = 0;

                                int damage = Helper.GetProjDamage(50, 60, 120);
                                NPC.NewProjectileDirectInAI<PurpleDash>(NPC.Center, Vector2.Zero, damage, 0
                                    , NPC.target, bigDashTime, NPC.whoAmI, 75);

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);

                                //DashFrame();

                                Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                                NPC.velocity = dir * 40;
                                NPC.rotation = NPC.velocity.ToRotation();
                                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                                isDashing = true;
                                //SetBackgroundLight(0.4f, bigDashTime - 3, 8);
                                var modifyer = new PunchCameraModifier(NPC.Center, dir * 2.3f, 14, 5, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                            }
                        }
                    }
                    break;
                case (int)LightningRaidState.BigDash://冲刺！冲刺！会朝向玩家拐弯的大冲
                    {
                        //UpdateAllOldCaches();

                        if (Timer == bigDashTime)
                        {
                            isDashing = false;
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                            NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                            currentSurrounding = false;
                        }
                        else if (Timer > bigDashTime)
                        {
                            NPC.velocity *= 0.8f;
                            //FlyingFrame();
                            if (Math.Abs(NPC.velocity.X) < 2f)
                            {
                                NPC.QuickSetDirection();
                                NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                            }
                        }
                        Timer++;
                        //int delayTime = 40;
                        //if (Main.getGoodWorld)
                        //    delayTime = 15;
                        //if (Timer > bigDashTime + delayTime)
                        //    ResetStates();
                    }
                    break;
            }

            return false;
        }
    }
}
