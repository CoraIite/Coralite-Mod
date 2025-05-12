using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class  ZacurrentDragon
    {
        /// <summary>
        /// 使用<see cref="Recorder"/>记录使用电球的样子
        /// </summary>
        public bool ElectricBall()
        {
            switch (SonState)
            {
                default:
                case 0://准备
                    {
                        const int ReadyTime = 35;

                        Vector2 pos = GetMousePos();
                        float edge = 140 - (60 * Math.Clamp(Timer / ReadyTime, 0, 1));
                        edge /= 2;
                        Vector2 center = pos + Helper.NextVec2Dir(edge - 1, edge);
                        Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                            (pos - center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 4),
                            newColor: ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;

                        NPC.QuickSetDirection();

                        //追踪玩家
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                        if (xLength > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 6f, 0.15f, 0.4f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.35f, 15, 0.9f);
                        else if (yLength > 70)
                        {
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4f, 0.15f, 0.3f, 0.95f);
                            FlyingFrame();
                        }
                        else
                        {
                            NPC.velocity.Y *= 0.95f;
                            FlyingFrame();
                        }

                        SetRotationNormally();

                        Timer++;
                        if (NPC.frame.Y == 0 && Timer > ReadyTime)
                        {
                            SonState=1;
                            Timer = 0;
                        }
                    }
                    return false;
                case 1://挥下翅膀
                    {
                        NPC.velocity *= 0.96f;
                        NPC.QuickSetDirection();
                        TurnToNoRot();

                        if (NPC.frame.Y != 4)
                            FlyingFrame();
                        else
                        {
                            Timer++;
                            if (Timer > 10)
                            {
                                SonState++;
                                Timer = 0;

                                NPC.frame.Y = 0;
                                OpenMouse = true;
                                canDrawShadows = true;
                                ResetAllOldCaches();
                            }
                        }
                    }
                    return false;
                case 2://射电球
                    {
                        Fly();
                        UpdateAllOldCaches();
                        SetSpriteDirectionFoTarget();
                        TurnToNoRot();

                        if (Timer % 20 == 0)
                        {
                            Helper.PlayPitched(CoraliteSoundID.TeslaTurret_Electric_NPCHit53, NPC.Center);
                            int damage = Helper.GetProjDamage(60, 80, 120);
                            Vector2 mousePos = GetMousePos();
                            NPC.NewProjectileDirectInAI<PurpleLightningBall>(mousePos, (Target.Center - mousePos).SafeNormalize(Vector2.Zero) * 4
                                , damage, 0, NPC.target);
                            NPC.velocity -= (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 3;
                        }

                        Timer++;
                        if (Timer > 20)
                        {
                            FlyingFrame();
                        }
                        if (Timer > 20 * 4)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    return false;
                case 3://射交错电球
                case 4://射旋转电球
                    {
                        Fly();
                        FlyingFrame();
                        UpdateAllOldCaches();
                        SetSpriteDirectionFoTarget();
                        TurnToNoRot();

                        Timer++;

                        const int readyTime = 40;

                        if (Timer < readyTime)
                        {
                            Vector2 pos = GetMousePos();
                            float edge = 140 - (60 * Math.Clamp(Timer / readyTime, 0, 1));
                            edge /= 2;
                            Vector2 center = pos + Helper.NextVec2Dir(edge - 1, edge);
                            Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                                (pos - center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 4),
                                newColor: ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }
                        if (Timer == readyTime)
                        {
                            int damage = Helper.GetProjDamage(100, 125, 145);

                            //预判
                            Vector2 targetPos = Target.Center;
                            if (Target.velocity.Length()>4)
                                targetPos += Target.velocity.SafeNormalize(Vector2.Zero) * 340;
                            Vector2 dir = (targetPos - GetMousePos()).SafeNormalize(Vector2.Zero);

                            switch (Recorder)
                            {
                                default:
                                case 0://3重电球
                                    {
                                        for (int i = -1; i < 2; i++)
                                        {
                                            NPC.NewProjectileDirectInAI<PurpleLightningBall>(GetMousePos()
                                                , dir.RotatedBy(i * 0.35f) * 3
                                                , damage, 0, NPC.target);
                                        }

                                        for (int i = -1; i < 2; i += 2)
                                        {
                                            NPC.NewProjectileDirectInAI<PurpleLightningBall>(GetMousePos()
                                                , dir.RotatedBy(i * 0.15f)
                                                , damage, 0, NPC.target);
                                        }
                                    }
                                    break;
                                case 1://单电球+直线链球
                                    {
                                        for (int i = -1; i < 2; i++)
                                        {
                                            NPC.NewProjectileDirectInAI<PurpleLightningBall>(GetMousePos()
                                                , dir.RotatedBy(i * 0.35f) * 3
                                                , damage, 0, NPC.target);
                                        }
                                        NPC.NewProjectileDirectInAI<ZacurrentChainBall>(GetMousePos()
                                            , dir * 7
                                            , damage, 0, NPC.target, 0);
                                    }
                                    break;
                                case 2://旋转链球
                                    {
                                        NPC.NewProjectileDirectInAI<ZacurrentChainBall>(GetMousePos()
                                            , dir * 7
                                            , damage, 0, NPC.target, 1);

                                        for (int i = -1; i < 2; i += 2)
                                        {
                                            NPC.NewProjectileDirectInAI<PurpleLightningBall>(GetMousePos()
                                                , dir.RotatedBy(i * 0.15f)
                                                , damage, 0, NPC.target);
                                        }
                                    }
                                    break;
                            }

                            NPC.velocity -= (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 3;
                            Helper.PlayPitched("Electric/ElectricShoot", 0.4f, -0.1f, NPC.Center);

                            if (!VaultUtils.isServer)
                            {
                                var modifyer = new PunchCameraModifier(NPC.Center
                                    , dir, 20, 15, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                            }
                        }
                        else if (Timer > readyTime + 30)
                        {
                            if (SonState == 3)
                            {
                                SonState++;
                                Timer = 0;
                                return false;
                            }

                            return true;
                        }
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

        public void LightingBallSetStartValue()
        {
            Recorder = Main.rand.Next(3);
        }
    }
}
