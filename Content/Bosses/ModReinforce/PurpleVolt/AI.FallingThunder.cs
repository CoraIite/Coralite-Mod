using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        public bool FallingThunder()
        {
            const int UpLength = 700;
            const int SmashDownTime = 12;

            switch (SonState)
            {
                default:
                case 0://先短暂向玩家靠近
                    {
                        const int chasingTime = 60 * 4;
                        NPC.QuickSetDirection();

                        //追踪玩家
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                        if (xLength < 400)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 24f, 0.5f, 0.6f, 0.95f);
                        else if (xLength > 600)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 24f, 0.5f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.4f, 15, 0.93f);
                        else if (yLength > 70)
                        {
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 20f, 0.4f, 0.6f, 0.95f);
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
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;

                            NPC.velocity = new Vector2(0, 20);
                        }
                    }
                    return false;
                case 1://向上飞
                    {
                        UpdateAllOldCaches();
                        if (Timer < 10)//向下运动
                        {
                            FlyingFrame();
                            NPC.QuickSetDirection();
                        }
                        else if (Timer == 10)
                        {
                            NPC.velocity = new Vector2(0, -50);
                            NPC.rotation = -MathHelper.PiOver2;
                            IsDashing=true;
                        }
                        else
                        {
                            selfAlpha = shadowAlpha = 1 - ((Timer - 10) / 20f);
                        }

                        Timer++;
                        if (Timer > 24)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            selfAlpha = shadowAlpha = 0;
                            NPC.dontTakeDamage = true;
                            canDrawShadows = false;
                            IsDashing = false;

                            Vector2 targetPos = Target.Center;
                            Recorder = targetPos.X;
                            Recorder2 = targetPos.Y;
                        }
                    }
                    return false;
                case 2://选择目标位置
                    {
                        const int AimingTime = 200;
                        const int ChasingTime = AimingTime - 45;

                        if (Timer % 45 == 0 && Timer < ChasingTime)
                        {
                            Vector2 pos = Target.Center;
                            if (Target.velocity.Length() > 4)
                                pos += Target.velocity.SafeNormalize(Vector2.Zero) * 250;

                            pos += new Vector2(((Timer / 45) % 2 == 0 ? -1 : 1) * Timer * 2, 260);
                            pos += Main.rand.NextVector2CircularEdge(80, 80);

                            int damage = Helper.GetProjDamage(100, 150, 200);
                            NPC.NewProjectileDirectInAI<PurpleSmallThunderFall>(pos, Vector2.Zero
                                , damage, 0, NPC.target, 75, NPC.whoAmI, 65);

                            Helper.PlayPitched(CoraliteSoundID.Ding_Item4, NPC.Center, pitch: 0.3f);
                        }

                        if (Timer % 60 == 0 && Timer < ChasingTime)
                        {
                            int damage = Helper.GetProjDamage(100, 150, 200);
                            NPC.NewProjectileDirectInAI<PurpleSmallThunderFall>(Target.Center, Vector2.Zero
                                , damage, 0, NPC.target, 60, NPC.whoAmI, 65);

                            Helper.PlayPitched(CoraliteSoundID.Ding_Item4, NPC.Center, pitch: 0.3f);
                        }

                        if (Timer < ChasingTime)
                        {
                            float factor1 = Timer / 80;
                            Vector2 targetPos = Target.Center + (Target.velocity * 28 * factor1);
                            targetPos = new Vector2(Recorder, Recorder2).MoveTowards(targetPos, 20);
                            Recorder = targetPos.X;
                            Recorder2 = targetPos.Y;
                            NPC.Center = targetPos + new Vector2(0, -UpLength);

                            //一开始的时候不追踪
                            if (Timer > AimingTime / 2)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Dust d = Dust.NewDustPerfect(targetPos, DustID.PortalBoltTrail
                                        , Helper.NextVec2Dir(2, 4)
                                        , newColor: ZacurrentPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                                    d.noGravity = true;
                                }
                                 
                                if (Main.rand.NextBool())
                                    ElectricParticle_PurpleFollow.Spawn(targetPos, Main.rand.NextVector2Circular(30, 30),
                                        () => new Vector2(Recorder, Recorder2), Main.rand.NextFloat(0.5f, 0.75f));
                            }
                        }
                        else if (Timer == ChasingTime)
                        {
                            Vector2 targetPos = new(Recorder, Recorder2);
                            NPC.Center = targetPos + new Vector2(0, -UpLength);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, targetPos);
                            for (int i = 0; i < 2; i++)
                            {
                                Dust d = Dust.NewDustPerfect(targetPos, DustID.PortalBoltTrail
                                    , Helper.NextVec2Dir(2, 4)
                                    , newColor: ZacurrentPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }
                        }
                        else
                        {
                            Vector2 targetPos = new(Recorder, Recorder2);
                            NPC.Center = targetPos + new Vector2(0, -UpLength);
                            float length = 30 + (60 * (Timer - ChasingTime) / (AimingTime - ChasingTime));
                            if (Main.rand.NextBool())
                                ElectricParticle_PurpleFollow.Spawn(targetPos, Main.rand.NextVector2Circular(length, length),
                                () => new Vector2(Recorder, Recorder2), Main.rand.NextFloat(0.5f, 0.75f));
                        }

                        Timer++;
                        if (Timer > AimingTime)
                        {
                            SonState++;
                            Timer = 0;
                            //生成闪电弹幕
                            Vector2 targetPos = new(Recorder, Recorder2);
                            int damage = Helper.GetProjDamage(180, 200, 260);

                            NPC.velocity = new Vector2(0, UpLength / (float)SmashDownTime);
                            NPC.rotation = NPC.velocity.ToRotation();
                            NPC.QuickSetDirection();
                            ResetAllOldCaches();
                            for (int i = 0; i < 3; i++)
                            {
                                NPC.NewProjectileDirectInAI<PurpleThunderFalling>(
                                    NPC.Center + new Vector2(-250 + (i * 500 / 3), Main.rand.Next(40, 300))
                                    , targetPos + new Vector2(0, 250), damage, 0, NPC.target, SmashDownTime + 8, NPC.whoAmI, 60);
                            }

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);

                            IsDashing = true;
                            canDrawShadows = true;
                            NPC.dontTakeDamage = false;
                            currentSurrounding = true;
                            SetBackgroundLight(0.25f, SmashDownTime);
                        }
                    }
                    return false;
                case 3://下落并带来落雷
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

                            IsDashing = false;
                            NPC.velocity *= 0;
                            NPC.QuickSetDirection();
                            TurnToNoRot(1);
                            NPC.frame.Y = 0;
                            var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY * 1.3f, 20, 22, 25, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }
                        else if (Timer < SmashDownTime + 30)
                        {
                            if (Timer % 2 == 0)
                            {
                                PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2Circular(NPC.width / 5, NPC.height / 5),
                                    Vector2.Zero, CoraliteContent.ParticleType<BigFog>(), ZacurrentPurple * Main.rand.NextFloat(0.5f, 0.8f),
                                    Main.rand.NextFloat(1.5f, 2f));
                                Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width * 0.8f, NPC.height * 0.8f);
                                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentPurple, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                            }

                            float factor = Helper.SqrtEase((Timer - SmashDownTime) / 30);
                            shadowScale = Helper.Lerp(1f, 2.5f, factor);
                            shadowAlpha = Helper.Lerp(1f, 0f, factor);
                        }
                        else if (Timer < SmashDownTime + 30 + 20)
                            FlyingFrame();
                        else
                            return true;

                        Timer++;
                    }
                    return false;
            }
        }
    }
}
