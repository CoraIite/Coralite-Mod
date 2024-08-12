using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void FallingThunderP1()
        {
            const int UpLength = 700;
            const int SmashDownTime = 12;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://先短暂向玩家靠近
                    {
                        const int chasingTime = 60 * 4;
                        NPC.QuickSetDirection();

                        //追踪玩家
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                        if (xLength < 400)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 18f, 0.35f, 0.6f, 0.95f);
                        else if (xLength > 600)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 18f, 0.35f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.4f, 15, 0.93f);
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
                        if (Timer > chasingTime || (xLength > 350 && yLength < 250 && Timer > 30))
                        {
                            SonState++;
                            Timer = 0;
                            DashFrame();
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;
                        }
                    }
                    break;
                case 1://吼叫
                    {
                        UpdateAllOldCaches();

                        NPC.QuickSetDirection();
                        TurnToNoRot();
                        NPC.velocity *= 0.9f;
                        if (Timer == 0 && NPC.frame.Y != 4)
                        {
                            FlyingFrame();
                            break;
                        }

                        if (Timer == 15)
                        {
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;
                            NPC.velocity *= 0;
                            Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, NPC.Center, pitch: 0.4f);
                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                        }
                        else if (Timer > 15 && Timer < 70)
                        {
                            Vector2 pos = NPC.Center + (NPC.rotation).ToRotationVector2() * 60 * NPC.scale;
                            if ((int)Timer % 10 == 0)
                                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Coralite.ThunderveinYellow, 0.2f);
                            if ((int)Timer % 20 == 0)
                                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.2f);
                        }

                        Timer++;
                        if (Timer > 80)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity = new Vector2(0, 8);
                            ResetAllOldCaches();
                            canDrawShadows = true;
                        }
                    }
                    break;
                case 2://向上飞
                    {
                        UpdateAllOldCaches();
                        if (Timer < 10)//向下运动
                        {
                            FlyingFrame();
                            NPC.QuickSetDirection();
                        }
                        else if (Timer == 10)
                        {
                            NPC.velocity = new Vector2(0, -45);
                            NPC.rotation = -MathHelper.PiOver2;
                            DashFrame();
                        }
                        else
                        {
                            selfAlpha = shadowAlpha = 1 - (Timer - 10) / 20f;
                        }

                        Timer++;
                        if (Timer > 30)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            selfAlpha = shadowAlpha = 0;
                            NPC.dontTakeDamage = true;
                            canDrawShadows = false;

                            Vector2 targetPos = Target.Center;
                            Recorder = targetPos.X;
                            Recorder2 = targetPos.Y;
                        }
                    }
                    break;
                case 3://选择目标位置
                    {
                        const int AimingTime = 80;
                        const int ChasingTime = 55;
                        if (Timer < ChasingTime)
                        {
                            float factor1 = Timer / 30;
                            Vector2 targetPos = Target.Center + Target.velocity * 28 * factor1;
                            targetPos = new Vector2(Recorder, Recorder2).MoveTowards(targetPos, 20);
                            Recorder = targetPos.X;
                            Recorder2 = targetPos.Y;
                            NPC.Center = targetPos + new Vector2(0, -UpLength);
                            for (int i = 0; i < 2; i++)
                            {
                                Dust d = Dust.NewDustPerfect(targetPos, DustID.PortalBoltTrail
                                    , Helper.NextVec2Dir(2, 4)
                                    , newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }

                            if (Main.rand.NextBool())
                                ElectricParticle_Follow.Spawn(targetPos, Main.rand.NextVector2Circular(30, 30),
                                    () => new Vector2(Recorder, Recorder2), Main.rand.NextFloat(0.5f, 0.75f));
                        }
                        else if (Timer == ChasingTime)
                        {
                            Vector2 targetPos = new Vector2(Recorder, Recorder2);
                            NPC.Center = targetPos + new Vector2(0, -UpLength);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, targetPos);
                            for (int i = 0; i < 2; i++)
                            {
                                Dust d = Dust.NewDustPerfect(targetPos, DustID.PortalBoltTrail
                                    , Helper.NextVec2Dir(2, 4)
                                    , newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }
                        }
                        else
                        {
                            Vector2 targetPos = new Vector2(Recorder, Recorder2);
                            NPC.Center = targetPos + new Vector2(0, -UpLength);
                            float length = 30 + 60 * (Timer - ChasingTime) / (AimingTime - ChasingTime);
                            if (Main.rand.NextBool())
                                ElectricParticle_Follow.Spawn(targetPos, Main.rand.NextVector2Circular(length, length),
                                () => new Vector2(Recorder, Recorder2), Main.rand.NextFloat(0.5f, 0.75f));
                        }

                        Timer++;
                        if (Timer > AimingTime)
                        {
                            SonState++;
                            Timer = 0;
                            //生成闪电弹幕
                            Vector2 targetPos = new Vector2(Recorder, Recorder2);
                            int damage = Helper.GetProjDamage(70, 80, 90);

                            NPC.velocity = new Vector2(0, UpLength / (float)SmashDownTime);
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.QuickSetDirection();
                            ResetAllOldCaches();
                            for (int i = 0; i < 3; i++)
                            {
                                NPC.NewProjectileDirectInAI<ThunderFalling>(
                                    NPC.Center + new Vector2(-250 + i * 500 / 3, Main.rand.Next(40, 300))
                                    , targetPos + new Vector2(0, 250), damage, 0, NPC.target, SmashDownTime + 8, NPC.whoAmI, 60);
                            }

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);

                            canDrawShadows = true;
                            NPC.dontTakeDamage = false;
                            currentSurrounding = true;
                            SetBackgroundLight(0.25f, SmashDownTime);
                        }
                    }
                    break;
                case 4://下落并带来落雷
                    {
                        UpdateAllOldCaches();
                        if (Timer < SmashDownTime)
                        {
                            selfAlpha += 1 / (float)SmashDownTime;
                            shadowAlpha = selfAlpha;
                        }
                        else if (Timer == SmashDownTime)
                        {
                            SetBackgroundLight(0.4f, 40);

                            NPC.velocity *= 0;
                            NPC.QuickSetDirection();
                            TurnToNoRot(1);
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;
                            var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY * 1.3f, 20, 22, 25, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }
                        else if (Timer < SmashDownTime + 30)
                        {
                            if (Timer % 2 == 0)
                            {
                                Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(NPC.width / 5, NPC.height / 5),
                                    Vector2.Zero, CoraliteContent.ParticleType<BigFog>(), Coralite.ThunderveinYellow * Main.rand.NextFloat(0.5f, 0.8f),
                                    Main.rand.NextFloat(1.5f, 2f));
                                Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width * 0.8f, NPC.height * 0.8f);
                                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                            }

                            float factor = Coralite.Instance.SqrtSmoother.Smoother((Timer - SmashDownTime) / 30);
                            shadowScale = Helper.Lerp(1f, 2.5f, factor);
                            shadowAlpha = Helper.Lerp(1f, 0f, factor);
                        }
                        else if (Timer < SmashDownTime + 30 + 50)
                        {
                            FlyingFrame();
                        }
                        else
                            ResetStates();
                        Timer++;
                    }
                    break;
            }
        }

        public void FallingThunderP2()
        {
            const int UpLength = 700;
            const int SmashDownTime = 12;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://先短暂向玩家靠近
                    {
                        const int chasingTime = 60 * 4;
                        NPC.QuickSetDirection();

                        //追踪玩家
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                        if (xLength < 400)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 18f, 0.35f, 0.6f, 0.95f);
                        else if (xLength > 600)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 18f, 0.35f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.4f, 15, 0.93f);
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
                        if (Timer > chasingTime || (xLength > 350 && yLength < 250 && Timer > 30))
                        {
                            StateRecorder = 0;
                            SonState++;
                            Timer = 0;
                            DashFrame();
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;
                        }
                    }
                    break;
                case 1://吼叫
                    {
                        UpdateAllOldCaches();

                        NPC.QuickSetDirection();
                        TurnToNoRot();
                        NPC.velocity *= 0.9f;
                        if (Timer == 0 && NPC.frame.Y != 4)
                        {
                            FlyingFrame();
                            break;
                        }

                        if (Timer == 15)
                        {
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;
                            NPC.velocity *= 0;
                            Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, NPC.Center, pitch: 0.4f);
                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                        }
                        else if (Timer > 15 && Timer < 70)
                        {
                            Vector2 pos = NPC.Center + (NPC.rotation).ToRotationVector2() * 60 * NPC.scale;
                            if ((int)Timer % 10 == 0)
                                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Coralite.ThunderveinYellow, 0.2f);
                            if ((int)Timer % 20 == 0)
                                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.2f);
                        }

                        Timer++;
                        if (Timer > 80)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity = new Vector2(0, 8);
                            ResetAllOldCaches();
                            canDrawShadows = true;
                        }
                    }
                    break;
                case 2://向上飞
                    {
                        UpdateAllOldCaches();
                        if (Timer < 10)//向下运动
                        {
                            FlyingFrame();
                            NPC.QuickSetDirection();
                        }
                        else if (Timer == 10)
                        {
                            NPC.velocity = new Vector2(0, -45);
                            NPC.rotation = -MathHelper.PiOver2;
                            DashFrame();
                        }
                        else
                        {
                            selfAlpha = shadowAlpha = 1 - (Timer - 10) / 20f;
                        }

                        Timer++;
                        if (Timer > 30)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            selfAlpha = shadowAlpha = 0;
                            NPC.dontTakeDamage = true;
                            canDrawShadows = false;

                            Vector2 targetPos = Target.Center;
                            Recorder = targetPos.X;
                            Recorder2 = targetPos.Y;
                        }
                    }
                    break;
                case 3://选择目标位置
                case 5://第二次选择
                case 7://第三次选择
                    {
                        const int AimingTime = 80;
                        const int ChasingTime = 55;
                        if (Timer < ChasingTime)
                        {
                            float factor1 = Timer / 30;
                            Vector2 targetPos = Target.Center + Target.velocity * 28 * factor1;
                            targetPos = new Vector2(Recorder, Recorder2).MoveTowards(targetPos, 20);
                            Recorder = targetPos.X;
                            Recorder2 = targetPos.Y;
                            NPC.Center = targetPos + new Vector2(0, -UpLength);
                            for (int i = 0; i < 2; i++)
                            {
                                Dust d = Dust.NewDustPerfect(targetPos, DustID.PortalBoltTrail
                                    , Helper.NextVec2Dir(2, 4)
                                    , newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }

                            if (Main.rand.NextBool())
                                ElectricParticle_Follow.Spawn(targetPos, Main.rand.NextVector2Circular(30, 30),
                                    () => new Vector2(Recorder, Recorder2), Main.rand.NextFloat(0.5f, 0.75f));
                        }
                        else if (Timer == ChasingTime)
                        {
                            Vector2 targetPos = new Vector2(Recorder, Recorder2);
                            NPC.Center = targetPos + new Vector2(0, -UpLength);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, targetPos);
                            for (int i = 0; i < 2; i++)
                            {
                                Dust d = Dust.NewDustPerfect(targetPos, DustID.PortalBoltTrail
                                    , Helper.NextVec2Dir(2, 4)
                                    , newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }
                        }
                        else
                        {
                            Vector2 targetPos = new Vector2(Recorder, Recorder2);
                            NPC.Center = targetPos + new Vector2(0, -UpLength);
                            float length = 30 + 60 * (Timer - ChasingTime) / (AimingTime - ChasingTime);
                            if (Main.rand.NextBool())
                                ElectricParticle_Follow.Spawn(targetPos, Main.rand.NextVector2Circular(length, length),
                                () => new Vector2(Recorder, Recorder2), Main.rand.NextFloat(0.5f, 0.75f));
                        }

                        Timer++;
                        if (Timer > AimingTime)
                        {
                            if (Main.rand.NextBool())
                            {
                                SonState++;
                            }
                            else
                            {
                                SonState = 8;
                            }

                            Timer = 0;
                            //生成闪电弹幕
                            Vector2 targetPos = new Vector2(Recorder, Recorder2);
                            int damage = Helper.GetProjDamage(100, 130, 150);

                            NPC.velocity = new Vector2(0, UpLength / (float)SmashDownTime);
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.QuickSetDirection();
                            ResetAllOldCaches();
                            for (int i = 0; i < 3; i++)
                            {
                                NPC.NewProjectileDirectInAI<StrongThunderFalling>(
                                    NPC.Center + new Vector2(-250 + i * 500 / 3, Main.rand.Next(40, 300))
                                    , targetPos + new Vector2(0, 250), damage, 0, NPC.target, SmashDownTime + 8, NPC.whoAmI, 50);
                            }

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);

                            canDrawShadows = true;
                            NPC.dontTakeDamage = false;
                            currentSurrounding = true;
                            SetBackgroundLight(0.25f, SmashDownTime);
                        }
                    }
                    break;
                case 4://第一次落雷后继续重复
                case 6://第二次落雷
                    {
                        UpdateAllOldCaches();
                        Timer++;

                        if (Timer < SmashDownTime)
                        {
                            selfAlpha += 1 / (float)SmashDownTime;
                            shadowAlpha = selfAlpha;
                        }
                        else if (Timer == SmashDownTime)
                        {
                            SetBackgroundLight(0.4f, 40);
                            var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY * 1.3f, 16, 20, 20, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }
                        else if (Timer < SmashDownTime * 2)
                        {
                            selfAlpha -= 1 / (float)SmashDownTime;
                            shadowAlpha = selfAlpha;
                        }
                        else
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            selfAlpha = shadowAlpha = 0;
                            NPC.dontTakeDamage = true;
                            canDrawShadows = false;

                            Vector2 targetPos = Target.Center;
                            Recorder = targetPos.X;
                            Recorder2 = targetPos.Y;

                        }
                    }
                    break;
                case 8://下落并带来落雷
                    {
                        UpdateAllOldCaches();
                        if (Timer < SmashDownTime)
                        {
                            selfAlpha += 1 / (float)SmashDownTime;
                            shadowAlpha = selfAlpha;
                        }
                        else if (Timer == SmashDownTime)
                        {
                            SetBackgroundLight(0.4f, 40);

                            NPC.velocity *= 0;
                            NPC.QuickSetDirection();
                            TurnToNoRot(1);
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;
                            var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY * 1.3f, 20, 22, 25, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }
                        else if (Timer < SmashDownTime + 30)
                        {
                            if (Timer % 2 == 0)
                            {
                                Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(NPC.width / 5, NPC.height / 5),
                                    Vector2.Zero, CoraliteContent.ParticleType<BigFog>(), Coralite.ThunderveinYellow * Main.rand.NextFloat(0.5f, 0.8f),
                                    Main.rand.NextFloat(1.5f, 2f));
                                Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width * 0.8f, NPC.height * 0.8f);
                                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                            }

                            float factor = Coralite.Instance.SqrtSmoother.Smoother((Timer - SmashDownTime) / 30);
                            shadowScale = Helper.Lerp(1f, 2.5f, factor);
                            shadowAlpha = Helper.Lerp(1f, 0f, factor);
                        }
                        else if (Timer < SmashDownTime + 30 + 25)
                        {
                            FlyingFrame();
                        }
                        else
                            ResetStates();
                        Timer++;
                    }
                    break;
            }
        }
    }
}
