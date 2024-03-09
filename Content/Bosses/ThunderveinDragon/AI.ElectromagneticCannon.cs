using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void ElectromagneticCannon()
        {
            const int burstTime = 100;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://与玩家拉近距离
                    {
                        const int chasingTime = 60 * 4;

                        Vector2 targetPos = Target.Center + new Vector2(0, -300);

                        NPC.direction = NPC.spriteDirection = targetPos.X > NPC.Center.X ? 1 : -1;
                        NPC.directionY = targetPos.Y > NPC.Center.Y ? 1 : -1;
                        SetRotationNormally();

                        //追踪玩家
                        GetLengthToTargetPos(targetPos, out float xLength, out float yLength);

                        if (xLength > 400)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 18f, 0.3f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.55f, 20, 0.9f);
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

                        Timer++;
                        if (Timer > chasingTime || (xLength < 400 && yLength < 100))
                        {
                            SonState++;
                            Timer = 0;
                            DashFrame();
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            isDashing = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;

                            currentSurrounding = true;

                            NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero)
                                .RotatedBy(-NPC.direction * MathHelper.PiOver2) * 20;
                        }
                    }
                    break;
                case 1://飞行一圈
                    {
                        const int rollingTime = 45;
                        const int rollingFactor = 60;

                        UpdateAllOldCaches();
                        NPC.velocity = NPC.velocity.RotatedBy(-NPC.spriteDirection * MathHelper.TwoPi / rollingFactor);
                        NPC.rotation = NPC.velocity.ToRotation();
                        Timer++;
                        if (Timer > rollingTime)
                        {
                            SonState++;
                            Timer = 0;
                            TurnToNoRot();
                            isDashing = false;
                        }
                    }
                    break;
                case 2://瞄准
                    {
                        NPC.velocity *= 0.9f;
                        UpdateAllOldCaches();

                        if (Timer < 10)
                        {
                            NPC.QuickSetDirection();
                            TurnToNoRot();
                            Recorder = (Target.Center - NPC.Center).ToRotation();
                        }

                        Vector2 pos2 = NPC.Center + (NPC.rotation - NPC.direction * 0.25f).ToRotationVector2() * 60;
                        Vector2 dir2 = Recorder.ToRotationVector2();
                        for (int i = 0; i < 3; i++)
                        {
                            Dust d = Dust.NewDustPerfect(pos2 + dir2 * Main.rand.NextFloat(20f, 1220f), DustID.PortalBoltTrail
                                , dir2.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: new Color(255, 202, 101),
                                Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }

                        if (NPC.frame.Y != 4)
                            FlyingFrame();
                        else
                        {
                            Timer++;
                            if (Timer > 35)
                            {
                                SonState++;
                                Timer = 0;
                                //生成吐息弹幕

                                NPC.velocity *= 0;
                                NPC.TargetClosest();
                                Vector2 pos = GetMousePos();
                                int damage = Helper.GetProjDamage(80, 100, 110);
                                NPC.NewProjectileDirectInAI<ElectromagneticCannon>(pos + Recorder.ToRotationVector2() * 1800, pos, damage, 0, NPC.target
                                    , burstTime, NPC.whoAmI, 85);

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                                SoundStyle st = CoraliteSoundID.PhantasmalDeathray_Zombie104;
                                st.Pitch = 0.3f;
                                SoundEngine.PlaySound(st, NPC.Center);
                                var modifyer = new PunchCameraModifier(NPC.Center, Recorder.ToRotationVector2() * 2, 24, 20, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                                canDrawShadows = true;
                                SetBackgroundLight(0.6f, burstTime * 3 / 4, 14);

                                ResetAllOldCaches();
                            }
                        }
                    }
                    break;
                case 3://吐息
                    {
                        UpdateAllOldCaches();

                        GetLengthToTargetPos(Target.Center, out float xLength, out _);

                        if (xLength > 50)
                            NPC.QuickSetDirection();
                        TurnToNoRot(1);
                        Recorder = Recorder.AngleTowards((Target.Center - GetMousePos()).ToRotation(), 0.014f);
                        FlyingFrame(true);
                        float factor = Coralite.Instance.SqrtSmoother.Smoother(Timer / burstTime);
                        shadowScale = Helper.Lerp(1f, 2f, factor);
                        shadowAlpha = Helper.Lerp(1f, 0f, factor);

                        Timer++;
                        if (Timer > 0 && Timer % 20 == 0)
                        {
                            var modifyer = new PunchCameraModifier(NPC.Center, Helper.NextVec2Dir(), 7, 12, 20, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }
                        if (Timer > burstTime)
                        {
                            canDrawShadows = false;
                            currentSurrounding = false;

                            Timer = 0;
                            SonState++;

                            NPC.QuickSetDirection();
                            TurnToNoRot();
                        }
                    }
                    break;
                case 4://后摇
                    {
                        TurnToNoRot();

                        FlyingFrame();
                        Timer++;
                        if (Timer > 25)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
