using Coralite.Content.Particles;
using Coralite.Content.Prefixes.FairyWeaponPrefixes;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        public bool DashDischarging()
        {
            const int bigDashTime = 60;
            const int burstTime = 60;

            switch (SonState)
            {
                default:
                case 0://先短暂远离准备冲刺
                    {
                        if (Timer == 0)
                        {
                            NPC.frame.Y = 1;
                            Timer = 1;
                        }

                        SetSpriteDirectionFoTarget();
                        SetRotationNormally(0.2f);

                        if (NPC.velocity.Length() < 8)
                            NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.65f;

                        //向后扇一下翅膀
                        if (++NPC.frameCounter > 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 7)
                            {
                                SonState=1;
                                Timer = 0;
                                ResetAllOldCaches();
                                canDrawShadows = true;
                                shadowScale = 1.2f;

                                Helper.PlayPitched("Electric/ElectricShoot", 0.4f, 0, NPC.Center);
                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                                IsDashing = true;

                                Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                                NPC.velocity = dir * 50;
                                NPC.rotation = NPC.velocity.ToRotation();
                                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                                //SetBackgroundLight(0.4f, bigDashTime - 3, 8);

                                WindCircle.Spawn(NPC.Center, -dir * 2, dir.ToRotation(), ZacurrentPurple
                                    , 0.6f, 3f, new Vector2(1.25f, 1f));
                            }
                        }
                    }
                    return false;
                case 1://开冲，直到与玩家距离小于一定值后停止
                    {
                        UpdateAllOldCaches();
                        GetLengthToTargetPos(Target.Center, out float xLength, out _);
                        if (xLength > 100)
                            NPC.QuickSetDirection();

                        NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 50;
                        NPC.rotation = NPC.velocity.ToRotation();

                        Timer++;
                        if (Timer > bigDashTime || Vector2.Distance(Target.Center, NPC.Center) < 200)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0.1f;
                            NPC.frame.Y = 0;
                            NPC.frameCounter = 0;
                            IsDashing = false;

                            Helper.PlayPitched("Electric/Charge", 1f, 0, NPC.Center);
                        }
                    }
                    return false;
                case 2://蓄力！
                    {
                        UpdateAllOldCaches();
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);
                        NPC.QuickSetDirection();
                        TurnToNoRot(0.2f);

                        if (xLength > 100)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 7.3f, 0.28f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (yLength > 70)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 5f, 0.3f, 0.5f, 0.95f);
                        else
                            NPC.velocity.Y *= 0.95f;

                        const int maxTime = (7 * 4) + 20;
                        Timer++;
                        float edge = 150 + ((540 - 150) * Math.Clamp(Timer / maxTime, 0, 1));
                        edge /= 2;
                        for (int i = 0; i < 4; i++)
                            SpawnDischargingDust(edge);

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
                                Recorder = (Target.Center - NPC.Center).ToRotation();

                                NPC.TargetClosest();
                                int damage = Helper.GetProjDamage(180, 200, 245);

                                if (!VaultUtils.isClient)
                                {
                                    NPC.NewProjectileDirectInAI<PurpleDischargingBurst>(NPC.Center, Vector2.Zero, damage, 0, NPC.target
                                        , burstTime, NPC.whoAmI);
                                }

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, NPC.Center);

                                if (!VaultUtils.isServer)
                                {
                                    var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY * 1.4f, 32, 26, 25, 1000);
                                    Main.instance.CameraModifiers.Add(modifyer);
                                }

                                OpenMouse = true;
                                canDrawShadows = true;
                                currentSurrounding = true;
                                SetBackgroundLight(0.5f, 25, 8);

                                ResetAllOldCaches();
                            }
                        }
                    }
                    return false;
                case 3://MD跟你爆了！！！！！！！！！！！
                    {
                        UpdateAllOldCaches();
                        TurnToNoRot(0.2f);
                        float factor = Coralite.Instance.SqrtSmoother.Smoother(Timer / burstTime);
                        shadowScale = Helper.Lerp(1f, 2.5f, (factor*3)%1);
                        shadowAlpha = Helper.Lerp(1f, 0f, factor);

                        NPC.velocity *= 0.985f;
                        if (NPC.frame.Y != 0)
                        {
                            if (++NPC.frameCounter > 1)
                            {
                                NPC.frameCounter = 0;
                                if (++NPC.frame.Y > 7)
                                    NPC.frame.Y = 0;
                            }
                        }

                        if (Timer < burstTime * 2 / 3)
                            Recorder = Recorder.AngleTowards((Target.Center - NPC.Center).ToRotation(), 0.01f);
                        SpawnThunderDusts(NPC.Center, 530, Recorder, Timer / burstTime);

                        Timer++;
                        if (Timer > burstTime)
                        {
                            Helper.PlayPitched("Electric/ElectricShoot", 0.4f, 0f, NPC.Center);
                            int damage = Helper.GetProjDamage(160, 180, 200);

                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 dir = (Recorder + (i * MathHelper.TwoPi / 5)).ToRotationVector2();
                                Vector2 pos = NPC.Center + (dir * 300);
                                NPC.NewProjectileInAI<PurpleElectricBallThunder>(pos + (dir * 600), pos, damage, 0, NPC.target,15,
                                    NPC.whoAmI, 70);
                            }

                            canDrawShadows = false;
                            currentSurrounding = false;

                            Timer = 0;
                            SonState++;
                        }
                    }
                    return false;
                case 4://后摇
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 25)
                            return true;
                    }
                    return false;
            }
        }

        public void SpawnDischargingDust(float edge)
        {
            if (VaultUtils.isServer)
                return;

            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(edge, edge);
            Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail
                , (pos - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(NPC.direction * MathHelper.PiOver2) * Main.rand.NextFloat(4f, 8f)
                , newColor: ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
            d.noGravity = true;
            pos = NPC.Center + Main.rand.NextVector2Circular(edge, edge);
            d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail
                , (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(4f, 8f)
                , newColor: ZacurrentPink);
            d.noGravity = true;
        }

        public void SpawnThunderDusts(Vector2 pos, float baseLength, float baseAngle, float factor)
        {
            float length = Helper.Lerp(baseLength, baseLength + 600, factor);
            for (int i = 0; i < 5; i++)
            {
                Vector2 dir = (baseAngle + (i * MathHelper.TwoPi / 5)).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(6, 6) + (dir * Main.rand.NextFloat(20, length)), DustID.PortalBoltTrail
                    , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: ZacurrentDragon.ZacurrentDustPurple,
                    Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

    }
}
