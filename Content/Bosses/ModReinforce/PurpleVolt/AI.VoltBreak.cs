using Coralite.Content.Particles;
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
        public bool VoltBreak()
        {
            const int BigDashSpeed = 55;
            const int bigDashTime = 26;

            switch (SonState)
            {
                default:
                case 0://飞到玩家某一侧
                    {
                        Vector2 targetPos = Target.Center + new Vector2(Recorder * 700, 0);
                        Vector2 dir = targetPos - NPC.Center;

                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 60;

                        NPC.velocity = targetRot.ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        SetSpriteDirectionFoTarget(targetPos);
                        SetRotationNormally();
                        FlyingFrame();
                        Timer++;
                        if (Timer > 60 * 3 || Vector2.Distance(targetPos, NPC.Center) < 80)
                        {
                            SonState = 1;
                            Timer = 0;
                            NPC.frame.Y = 8;
                            Helper.PlayPitched("Electric/Charge", 1f, 0, NPC.Center);
                        }
                    }
                    return false;
                case 1://准备动作
                    {
                        const int maxTime = 6 * 6;

                        SetSpriteDirectionFoTarget();
                        TurnToNoRot();

                        if (Timer < maxTime / 2)
                        {
                            Vector2 targetPos = Target.Center + new Vector2(Recorder * 650, 0);

                            GetLengthToTargetPos(targetPos, out float xLength, out float yLength);
                            if (xLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 8f, 0.3f, 0.6f, 0.95f);
                            else
                                NPC.velocity.X *= 0.95f;

                            //if (Timer < maxTime / 3)
                            {
                                if (NPC.directionY < 0)
                                    FlyingUp(0.35f, 15, 0.9f);
                                else if (yLength > 70)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 8f, 0.3f, 0.5f, 0.95f);
                                else
                                    NPC.velocity.Y *= 0.95f;
                            }
                        }

                        UpdateAllOldCaches();

                        if (++NPC.frameCounter > 6)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y--;
                        }

                        if (Timer < maxTime / 2)
                        {
                            Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.width / 2);
                            pos += Timer / (maxTime / 2) * new Vector2(-Recorder, 0) * 1000;
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
                        if (Timer > maxTime)
                        {
                            SonState = (int)LightningRaidState.BigDash;
                            Timer = 0;

                            IsDashing = true;
                            int damage = Helper.GetProjDamage(130, 160, 200);
                            NPC.NewProjectileDirectInAI<RedDash>(NPC.Center, Vector2.Zero, damage, 0
                                , NPC.target, bigDashTime, NPC.whoAmI, 14);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);

                            Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                            NPC.velocity = new Vector2(-Recorder * BigDashSpeed, 0);
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

                                WindCircle.Spawn(NPC.Center, -new Vector2(-Recorder, 0) * 2, dir.ToRotation(), ZacurrentPurple
                                    , 0.6f, 3f, new Vector2(1.25f, 1f));
                            }
                        }
                    }
                    return false;
                case 2://冲刺！冲刺！会朝向玩家拐弯的大冲
                    {
                        UpdateAllOldCaches();

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
                            if (Timer % 3 == 0)
                            {
                                int damage = Helper.GetProjDamage(100, 150, 200);
                                NPC.NewProjectileDirectInAI<PurpleSmallThunderFall>(NPC.Center + new Vector2(0, 400), Vector2.Zero
                                    , damage, 0, NPC.target, 35 + Timer, NPC.whoAmI, 65);
                            }
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

                        if (Timer > bigDashTime + delayTime)
                            return true;
                    }
                    return false;

            }
        }

        public void VoltBreakSetStartValue()
        {
            Recorder = Main.rand.NextFromList(-1, 1);
        }
    }
}
