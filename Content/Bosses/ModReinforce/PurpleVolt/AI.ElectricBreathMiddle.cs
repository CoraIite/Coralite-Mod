using Coralite.Helpers;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        /// <summary>
        /// 使用record2控制飞在玩家左边还是右边
        /// </summary>
        /// <returns></returns>
        public bool ElectricBreathMiddle()
        {
            const int BreathTime = 60;

            switch (SonState)
            {
                default:
                case 0://飞得靠近玩家
                    {
                        const int maxTime = 60 * 7;

                        return ElectricBreathFly(maxTime, -250, false);
                    }
                case 1://绕圈飞行
                    {
                        const int rollingTime = 45;
                        const int rollingFactor = 50;

                        if (Timer == 0)
                        {
                            NPC.FaceTarget();
                            ResetAllOldCaches();
                            IsDashing = true;
                            canDrawShadows = true;
                            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                            NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero)
                                .RotatedBy(-NPC.direction * MathHelper.PiOver2) * 28;
                            NPC.netUpdate = true;
                            Helper.PlayPitched("Electric/LightningBeam", 0.4f, 0, NPC.Center);
                        }

                        Timer++;
                        if (Timer < rollingTime)
                        {
                            UpdateAllOldCaches();
                            NPC.velocity = NPC.velocity.RotatedBy(-NPC.spriteDirection * MathHelper.TwoPi / rollingFactor);
                            NPC.rotation = NPC.velocity.ToRotation();

                        }
                        else
                        {
                            NPC.frame.Y = 0;
                            NPC.velocity *= 0.25f;
                            SonState = 2;
                            Timer = 0;
                            IsDashing = false;
                            currentSurrounding = true;
                            OpenMouse = true;
                            Recorder = (Target.Center - NPC.Center).ToRotation();
                            NPC.FaceTarget();
                            TurnToNoRot();
                        }
                    }
                    return false;
                case 2://蓄力
                    {
                        Vector2 targetPos2 = Target.Center + new Vector2(Recorder2 > 0 ? -400 : 400, -250);

                        GetLengthToTargetPos(targetPos2, out float xLength, out float yLength);

                        NPC.direction = targetPos2.X > NPC.Center.X ? 1 : -1;
                        NPC.directionY = targetPos2.Y > NPC.Center.Y ? 1 : -1;

                        if (xLength > 100)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 15f, 0.4f, 0.6f, 0.95f);
                        else if (xLength < 20)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 15f, 0.4f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.92f;

                        if (yLength > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.25f, 1f, 0.95f);
                        else
                            NPC.velocity.Y *= 0.9f;

                        UpdateAllOldCaches();
                        SetSpriteDirectionFoTarget();
                        SetRotationNormally();

                        BreathMouseDust();
                        const int maxTime = 35;

                        Vector2 mousePos = GetMousePos();
                        Vector2 currentPos = mousePos + Recorder.ToRotationVector2() * (Target.Center - NPC.Center).Length();
                        float aimRot = (currentPos.MoveTowards(Target.Center, 13) - mousePos).ToRotation();
                        Recorder = Recorder.AngleTowards(aimRot, 0.03f * (1 - Timer / maxTime));

                        Timer++;
                        if (Timer % 3 == 0)
                        {
                            Vector2 pos = GetMousePos();
                            Vector2 dir2 = Recorder.ToRotationVector2();
                            Vector2 targetPos = pos + dir2 * ((Target.Center - pos).Length() + 200);
                            if (Main.rand.NextBool())
                            {
                                Dust d = Dust.NewDustPerfect(pos + dir2 * Main.rand.NextFloat(20f, 150f), DustID.PortalBoltTrail
                                        , dir2.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: ZacurrentDustPurple,
                                        Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }

                            PurpleThunderParticle.Spawn(GetMousePos, (Recorder + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.NextFloat(25, 35)
                                , 6, 10, 7, 50, Main.rand.NextFromList(ZacurrentPurple, ZacurrentPink));

                            if (Main.rand.NextBool(3))
                            {
                                float speed = Main.rand.NextFloat(45, 60);
                                Vector2 dir = Vector2.UnitY.RotateByRandom(-0.5f, 0.5f);
                                Vector2 rand = -dir * (speed * 12);
                                PurpleThunderParticle.Spawn(() => NPC.Center + rand, dir * speed
                                    , 12, 5, 25, 70, Main.rand.NextFromList(ZacurrentPurple, ZacurrentPink));
                            }
                        }

                        //飞行帧图
                        if (Timer % 3 == 0)
                        {
                            if (NPC.frame.Y < 4)
                            {
                                NPC.frame.Y++;
                            }
                        }

                        if (Timer > maxTime)
                        {
                            SonState = 3;
                            Timer = 0;
                            NPC.frame.Y = 0;
                            if (!VaultUtils.isServer)
                            {
                                var modifyer = new PunchCameraModifier(NPC.Center, Recorder.ToRotationVector2(), 75, 25, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                                SetBackgroundLight(0.4f, 25, 8);
                            }

                            Vector2 dir = Recorder.ToRotationVector2();

                            if (!VaultUtils.isClient)
                            {
                                Vector2 pos = GetMousePos();
                                int damage = Helper.GetProjDamage(90, 110, 130);

                                NPC.NewProjectileDirectInAI<PurpleElectricBreath>(pos + dir * ((Target.Center - pos).Length() + 350), pos, damage, 0, NPC.target
                                , BreathTime, NPC.whoAmI, 70);
                                NPC.NewProjectileDirectInAI<PurpleElectricBall>(pos + dir * ((Target.Center - pos).Length() + 350), pos, damage, 0, NPC.target
                                , NPC.whoAmI, Main.rand.NextFloat(MathHelper.TwoPi), 35);
                            }

                            NPC.velocity = -dir * 8;
                        }
                    }
                    return false;
                case 3://吐息
                    {
                        int restTime = Helper.ScaleValueForDiffMode(40, 35, 30, 20);

                        Timer++;
                        if (Timer < BreathTime + 10)
                        {
                            NPC.velocity *= 0.95f;
                            UpdateAllOldCaches();
                            TurnToNoRot();
                        }
                        else if (Timer == BreathTime + 10)
                        {
                            currentSurrounding = false;
                            canDrawShadows = false;
                            OpenMouse = false;
                        }
                        else if (Timer < BreathTime + 10 + restTime)
                        {
                            FlyingFrame();
                            Fly();
                        }
                        else
                            return true;
                    }
                    return false;
            }

            void Fly()
            {
                float distance = NPC.Center.Distance(Target.Center);
                if (distance < 400)
                {
                    if (NPC.velocity.Length() < 8)
                        NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.65f;
                }
                else if (distance > 900)
                {
                    if (NPC.velocity.Length() < 20)
                        NPC.velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.65f;
                }
                else
                    NPC.velocity *= 0.95f;
            }
        }

        public void ElectricBreathMiddleSetStartValue()
        {
            Recorder2 = Main.rand.Next(2);
        }
    }
}
