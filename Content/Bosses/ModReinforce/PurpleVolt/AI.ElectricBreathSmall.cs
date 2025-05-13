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
        public bool ElectricBreathSmall()
        {
            const int BreathTime = 30;

            switch (SonState)
            {
                default:
                case 2:
                case 0://飞得靠近玩家
                    {
                        const int maxTime = 60 * 6;

                        return ElectricBreathFly(maxTime, -MathF.Sin(MathF.Sqrt(Timer / maxTime) * MathHelper.Pi) * 200);
                    }
                case 3:
                case 1://吐息
                    {
                        SetRotationNormally();
                        Recorder = Recorder.AngleTowards((Target.Center - NPC.Center).ToRotation(), 0.005f);

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
                            NPC.velocity *= 0.9f;

                        int ReadyTime = Helper.ScaleValueForDiffMode(35, 33, 29, 26);

                        Timer++;
                        if (Timer == 1)
                        {
                            Helper.PlayPitched(CoraliteSoundID.TeslaTurret_Electric_NPCHit53, NPC.Center);
                        }
                        else if (Timer < ReadyTime)//短暂的准备时间
                        {
                            BreathMouseDust();
                            FlyingFrame();
                            if (Timer % 5 == 0)
                            {
                                PurpleThunderParticle.Spawn(GetMousePos, (Recorder + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.NextFloat(12, 18)
                                    , 9, 15, 9, 20, Main.rand.NextFromList(ZacurrentPurple, ZacurrentPink));
                            }
                        }
                        else if (Timer == ReadyTime)
                        {
                            currentSurrounding = true;
                            //吐息
                            ElectricSound();
                            SoundEngine.PlaySound(CoraliteSoundID.BubbleShield_Electric_NPCHit43, NPC.Center);
                            if (!VaultUtils.isServer)
                            {
                                var modifyer = new PunchCameraModifier(NPC.Center, Recorder.ToRotationVector2(), 20, 20, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                            }

                            Vector2 dir = Recorder.ToRotationVector2();

                            if (!VaultUtils.isClient)
                            {
                                Vector2 pos = GetMousePos();
                                int damage = Helper.GetProjDamage(90, 110, 130);

                                NPC.NewProjectileDirectInAI<PurpleElectricBreath>(pos + dir * ((Target.Center - pos).Length() + 200), pos, damage, 0, NPC.target
                                , BreathTime, NPC.whoAmI, 70);
                            }

                            NPC.velocity = -dir * 8;
                        }
                        else if (Timer < BreathTime + ReadyTime + 15)//吐息中
                        {
                            NPC.velocity *= 0.95f;
                            FlyingFrame();
                        }
                        else
                        {
                            currentSurrounding = false;
                            OpenMouse = false;
                            //只能使用2次
                            Recorder2 = -Recorder2;
                            if (SonState > 2)
                                return true;

                            Timer = 0;
                            SonState = 0;
                        }
                    }
                    return false;
            }
        }

        private bool ElectricBreathFly(int maxTime,float targetY, bool mouseDust = true)
        {
            if (Timer == 0)
                Recorder = (Target.Center - NPC.Center).ToRotation();

            Recorder = Recorder.AngleTowards((Target.Center - NPC.Center).ToRotation(), 0.08f);

            Vector2 targetPos = Target.Center + new Vector2(Recorder2 > 0 ? -400 : 400, targetY);

            //追踪玩家
            GetLengthToTargetPos(targetPos, out float xLength, out float yLength);

            NPC.direction = targetPos.X > NPC.Center.X ? 1 : -1;
            NPC.directionY = targetPos.Y > NPC.Center.Y ? 1 : -1;
            SetSpriteDirectionFoTarget();

            SetRotationNormally();

            if (xLength > 300)
                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 24f, 0.4f, 0.6f, 0.95f);
            else if (xLength < 100)
                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 24f, 0.4f, 0.6f, 0.95f);
            else
            {
                NPC.velocity.X *= 0.92f;
                Timer += 20;
            }

            if (NPC.directionY < 0)
                FlyingUp(0.9f, 20, 0.85f);
            else if (yLength > 50)
            {
                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 16f, 0.45f, 1f, 0.95f);
                FlyingFrame();
            }
            else
            {
                NPC.velocity.Y *= 0.9f;
                FlyingFrame();
            }

            if (mouseDust && NPC.Distance(Target.Center) < 750)
            {
                OpenMouse = true;
                BreathMouseDust();
                if (Timer % 2 == 0 && Main.rand.NextBool(3))
                    ElectricParticle_PurpleFollow.Spawn(targetPos, Main.rand.NextVector2CircularEdge(28, 28)
                        , () =>
                        {
                            Vector2 pos = GetMousePos();
                            Vector2 dir = Recorder.ToRotationVector2();
                            return targetPos = pos + dir * ((Target.Center - pos).Length() + 200);
                        });
            }
            else
                OpenMouse = false;

            Timer++;
            if (Timer > maxTime)
            {
                //如果距离太远就直接结束阶段
                if (NPC.Distance(Target.Center) > 1400)
                    return true;

                SonState = 1;
                Timer = 0;
                OpenMouse = true;
            }

            return false;
        }

        /// <summary>
        /// 吐息时的嘴巴粒子
        /// </summary>
        private void BreathMouseDust()
        {
            if (VaultUtils.isServer)
                return;

            Vector2 pos = GetMousePos();
            Vector2 dir = Recorder.ToRotationVector2();
            Vector2 targetPos = pos + dir * ((Target.Center - pos).Length() + 200);
            //for (int i = 0; i < 2; i++)
            //{
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(pos + dir * Main.rand.NextFloat(20f, 150f), DustID.PortalBoltTrail
                        , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: ZacurrentDustPurple,
                        Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

            if (Main.rand.NextBool())
            {

                Dust d = Dust.NewDustPerfect(targetPos + Main.rand.NextVector2Circular(32, 32), DustID.PortalBoltTrail
                    , -dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: ZacurrentDustPurple
,
                    Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

            //}
        }

        public void ElectricBreathSmallSetStartValue()
        {
            Recorder2 = -MathF.Sign(Target.Center.X - NPC.Center.X);
        }
    }
}
