using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        /// <summary>
        /// 使用<see cref="Recorder"/> 记录第一次短冲的冲刺次数，<see cref="Recorder2"/>记录多次冲刺
        /// </summary>
        /// <returns></returns>
        public bool LightningRaidVolt()
        {
            const int smallDashTime = 7;
            const int bigDashTime = 26;

            const int BigDashSpeed = 55;

            switch (SonState)
            {
                default:
                    return true;
                case (int)LightningRaidState.SmallDash://小冲刺
                    {
                        const int smallDashSpeed = 60;

                        if (Timer == 0)
                        {
                            //生成弹幕并随机速度方向
                            int damage = Helper.GetProjDamage(20, 30, 70);
                            NPC.NewProjectileDirectInAI<RedDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, smallDashTime - 1, NPC.whoAmI, 6);

                            ElectricSound();
                            float targetrot = (Target.Center - NPC.Center).ToRotation();

                            //距离小于700那就不会朝向玩家冲刺
                            if (Vector2.Distance(NPC.Center, Target.Center) < 700)
                                targetrot += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(0.7f, 1.2f);
                            NPC.velocity = targetrot.ToRotationVector2() * smallDashSpeed;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                            IsDashing = true;
                        }
                        else if (Timer > smallDashTime / 4 && Timer < smallDashTime)//根据目标位置偏移一下
                        {
                            float distance = NPC.Center.Distance(Target.Center);
                            if (distance < 500)
                            {
                                float factor = 1 - Math.Clamp(distance / 600, 0.01f, 1);

                                //目标方向
                                float targetDir = (Target.Center - NPC.Center).ToRotation() + MathHelper.Pi;
                                float velocityDir = NPC.velocity.ToRotation().AngleTowards(targetDir, 0.35f * factor);
                                NPC.velocity = velocityDir.ToRotationVector2() * smallDashSpeed;
                                NPC.rotation = velocityDir;
                            }
                        }

                        UpdateAllOldCaches();

                        Timer++;
                        if (Timer > smallDashTime)
                        {
                            Timer = 0;

                            Recorder--;
                            if (Recorder < 1)//结束短冲
                                SonState = (int)LightningRaidState.ReadyBigDash;

                            NPC.velocity *= 0;
                            NPC.frameCounter = 0;
                            NPC.frame.Y = 7;
                            IsDashing = false;
                        }
                    }
                    return false;
                case (int)LightningRaidState.ReadyBigDash://扇翅膀向后飞行
                    {
                        const int maxTime = 6 * 9;

                        NPC.QuickSetDirection();
                        SetRotationNormally(0.2f);

                        float distance = NPC.Center.Distance(Target.Center);
                        if (distance < 800)
                        {
                            if (NPC.velocity.Length() < 8)
                                NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.65f;
                        }
                        else if (distance > 1000)
                        {
                            if (NPC.velocity.Length() < 8)
                                NPC.velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.65f;
                        }
                        else
                            NPC.velocity *= 0.9f;

                        UpdateAllOldCaches();

                        if (++NPC.frameCounter > 6)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y--;
                        }

                        if (Timer < maxTime / 2)
                        {
                            Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.width / 2);
                            pos += Timer / (maxTime / 2) * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 1000;
                            RedElectricParticle(pos);
                        }

                        if (Main.rand.NextBool())
                        {
                            float dis2 = Helper.Lerp(80, 250, Timer / maxTime);
                            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(dis2, dis2);

                            RedElectricParticle(pos);
                        }

                        shadowScale = Helper.Lerp(1, 2.5f, Timer / maxTime);
                        shadowAlpha = Helper.Lerp(1, 0, Timer / maxTime);

                        Timer++;
                        if (Timer > maxTime || NPC.frame.Y < 1)
                        {
                            SonState = (int)LightningRaidState.BigDash;
                            Timer = 0;

                            IsDashing = true;
                            int damage = Helper.GetProjDamage(150, 175, 200);
                            NPC.NewProjectileDirectInAI<RedDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, bigDashTime, NPC.whoAmI, 14);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);

                            Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                            NPC.velocity = dir * BigDashSpeed;
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                            IsDashing = true;
                            SetBackgroundLight(0.4f, bigDashTime - 3, 8);
                            shadowScale = 1.1f;
                            shadowAlpha = 1;

                            if (!VaultUtils.isServer)
                            {
                                var modifyer = new PunchCameraModifier(NPC.Center, dir * 2.3f, 16, 5, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);

                                WindCircle.Spawn(NPC.Center, -dir * 2, dir.ToRotation(), ZacurrentPurple
                                    , 0.6f, 3f, new Vector2(1.25f, 1f));
                            }
                        }
                    }
                    return false;
                case (int)LightningRaidState.BigDash://冲刺！冲刺！会朝向玩家拐弯的大冲
                    {
                        UpdateAllOldCaches();

                        if (Timer > bigDashTime * 0.4f && Timer < bigDashTime * 0.8f)//根据目标位置偏移一下
                        {
                            float distance = NPC.Center.Distance(Target.Center);
                            float factor = Math.Clamp(distance / 1000, 0.01f, 1);

                            //目标方向
                            float targetDir = (Target.Center - NPC.Center).ToRotation();
                            float velocityDir = NPC.velocity.ToRotation().AngleTowards(targetDir, 0.1f * factor);
                            NPC.velocity = velocityDir.ToRotationVector2() * BigDashSpeed;
                            NPC.rotation = velocityDir;
                        }

                        if (Timer < bigDashTime)
                        {
                            const float ZMoveAngle = 0.3f;

                            //折返冲刺
                            if (Timer == bigDashTime * 4 / 20)
                            {
                                NPC.velocity = NPC.velocity.RotatedBy(ZMoveAngle);
                            }

                            if (Timer == bigDashTime * 7 / 20)
                            {
                                NPC.velocity = NPC.velocity.RotatedBy(-ZMoveAngle * 2);
                            }

                            if (Timer == bigDashTime * 13 / 20)
                            {
                                NPC.velocity = NPC.velocity.RotatedBy(ZMoveAngle * 2);
                            }

                            if (Timer == bigDashTime * 16 / 20)
                            {
                                NPC.velocity = NPC.velocity.RotatedBy(-ZMoveAngle);
                            }

                            NPC.rotation = NPC.velocity.ToRotation();
                        }
                        else if (Timer == bigDashTime)
                        {
                            IsDashing = false;
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                            NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                            currentSurrounding = false;
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

                        //休息时间
                        int delayTime = Helper.ScaleValueForDiffMode(60, 40, 30, 15);
                        //还能接着冲的话就不休息
                        if (Recorder2 > 0)
                            delayTime = 5;

                        if (Timer > bigDashTime + delayTime)
                        {
                            Timer = 0;
                            //冲刺次数
                            Recorder2--;
                            if (Recorder2 < 1)
                                return true;
                            else
                            {
                                //重新开始新一轮的冲刺
                                Recorder = 2;
                                float distance = Vector2.Distance(NPC.Center, Target.Center);
                                if (distance > 700)
                                    Recorder++;

                                SonState = (int)LightningRaidState.SmallDash;
                                return false;
                            }
                        }
                    }
                    return false;
            }
        }

        public static void RedElectricParticle(Vector2 pos)
        {
            if (Main.rand.NextBool())
                PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
            else
                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentDustRed, Scale: Main.rand.NextFloat(0.1f, 0.3f));
        }
    }
}
