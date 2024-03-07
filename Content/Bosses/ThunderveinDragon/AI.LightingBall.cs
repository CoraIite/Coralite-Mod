using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void LightingBall()
        {
            const int burstTime = 25;
            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://蓄力
                    {
                        const int ReadyTime = 25;

                        Vector2 pos = GetMousePos();
                        float edge = 140 - 60 * Math.Clamp(Timer / ReadyTime, 0, 1);
                        edge /= 2;
                        Vector2 center = pos + Helper.NextVec2Dir(edge - 1, edge);
                        Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                            (pos - center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 4),
                            newColor: new Color(255, 202, 101), Scale: Main.rand.NextFloat(1f, 1.5f));
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
                            FlyingFrame(true);
                        }
                        else
                        {
                            NPC.velocity.Y *= 0.95f;
                            FlyingFrame(true);
                        }

                        SetRotationNormally();

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
                                int damage = Helper.GetProjDamage(80, 100, 120);
                                NPC.NewProjectileDirectInAI<LightningBall>(GetMousePos(), (Target.Center - GetMousePos()).SafeNormalize(Vector2.Zero) * 2
                                    , damage, 0, NPC.target);

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                                canDrawShadows = true;
                                ResetAllOldCaches();
                            }
                        }
                    }
                    break;
                case 2://射🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍
                    {
                        UpdateAllOldCaches();
                        float factor = Coralite.Instance.SqrtSmoother.Smoother(Timer / burstTime);
                        shadowScale = Helper.Lerp(1f, 1.5f, factor);
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
                            Timer = 0;
                            SonState++;
                        }
                    }
                    break;
                case 3://短暂后摇
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 10)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
