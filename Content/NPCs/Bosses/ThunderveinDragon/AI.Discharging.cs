using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void Discharging()
        {
            const int burstTime = 35;

            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://蓄力，微微靠近玩家
                    {
                        //由于已经是离玩家很近才会使用的招式所以直接微微靠近
                        const int ReadyTime = 40;
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
                        float edge = 400 + 220 * Math.Clamp(Timer / ReadyTime, 0, 1);
                        edge /= 2;
                        for (int i = 0; i < 4; i++)
                        {
                            SpawnDischargingDust(edge);
                        }
                        Timer++;
                        if (NPC.frame.Y == 0 && Timer > ReadyTime)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://挥下翅膀
                    {
                        NPC.velocity *= 0.96f;
                        NPC.QuickSetDirection();
                        TurnToNoRot();
                        float edge = (40 + 580) / 2;
                        for (int i = 0; i < 4; i++)
                        {
                            SpawnDischargingDust(edge);
                        }

                        if (NPC.frame.Y != 4)
                            FlyingFrame();
                        else
                        {
                            Timer++;
                            if (Timer > 10)
                            {
                                SonState++;
                                Timer = 0;
                                //生成爆炸弹幕

                                NPC.TargetClosest();
                                int damage = Helper.GetProjDamage(100, 130, 180);
                                if (Phase == 1)
                                    NPC.NewProjectileDirectInAI<DischargingBurst>(NPC.Center, Vector2.Zero, damage, 0, NPC.target
                                        , burstTime, NPC.whoAmI);
                                else
                                    NPC.NewProjectileDirectInAI<StrongDischargingBurst>(NPC.Center, Vector2.Zero, damage, 0, NPC.target
                                        , burstTime, NPC.whoAmI);

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, NPC.Center);
                                canDrawShadows = true;
                                currentSurrounding = true;
                                SetBackgroundLight(0.5f, burstTime - 3, 8);
                                var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY * 1.4f, 26, 26, 25, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);

                                ResetAllOldCaches();
                            }
                        }
                    }
                    break;
                case 2://爆！！！！！！！！！
                    {
                        UpdateAllOldCaches();
                        float factor = Coralite.Instance.SqrtSmoother.Smoother(Timer / burstTime);
                        shadowScale = Helper.Lerp(1f, 2.5f, factor);
                        shadowAlpha = Helper.Lerp(1f, 0f, factor);

                        if (NPC.frame.Y != 0)
                        {
                            NPC.frame.X = 1;

                            if (++NPC.frameCounter > 1)
                            {
                                NPC.frameCounter = 0;
                                if (++NPC.frame.Y > 7)
                                    NPC.frame.Y = 0;
                            }
                        }
                        Timer++;
                        if (Timer > burstTime)
                        {
                            canDrawShadows = false;
                            currentSurrounding = false;

                            Timer = 0;
                            SonState++;
                        }
                    }
                    break;
                case 3://短暂后摇
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 25)
                            ResetStates();
                    }
                    break;
            }
        }

        public void SpawnDischargingDust(float edge)
        {
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(edge, edge);
            Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail
                , (pos - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(NPC.direction * MathHelper.PiOver2) * Main.rand.NextFloat(4f, 8f)
                , newColor: Coralite.Instance.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
            d.noGravity = true;
            pos = NPC.Center + Main.rand.NextVector2Circular(edge, edge);
            d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail
                , (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(4f, 8f)
                , newColor: Coralite.Instance.ThunderveinYellow);
            d.noGravity = true;
        }
    }
}
