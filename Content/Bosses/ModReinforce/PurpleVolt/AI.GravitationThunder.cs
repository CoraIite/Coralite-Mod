using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        public bool GravitationThunder(int maxTime=60*5)
        {
            const int burstTime = 40;
            switch (SonState)
            {
                default:
                case 0://扇动翅膀并向玩家飞
                    {
                        const int ReadyTime = 45;

                        Vector2 pos = GetMousePos();
                        float edge = 240 - (140 * Math.Clamp(Timer / ReadyTime, 0, 1));
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
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 8f, 0.3f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.35f, 15, 0.9f);
                        else if (yLength > 70)
                        {
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 5f, 0.3f, 0.5f, 0.95f);
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
                            SonState = 1;
                            Timer = 0;
                        }
                    }
                    return false;
                case 1://吐出引力雷球
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
                                SonState = 2;
                                Timer = 0;
                                //生成雷球弹幕
                                int damage = Helper.GetProjDamage(150, 200, 250);

                                if (!VaultUtils.isClient)
                                    NPC.NewProjectileDirectInAI<PurpleGravitationThunderBall>(GetMousePos(), (Target.Center - GetMousePos()).SafeNormalize(Vector2.Zero) * 2
                                        , damage, 0, NPC.target,maxTime);

                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, NPC.Center);
                                canDrawShadows = true;
                                ResetAllOldCaches();
                            }
                        }
                    }
                    return false;
                case 2:
                    {
                        UpdateAllOldCaches();
                        float factor = Helper.SqrtEase(Timer / burstTime);
                        shadowScale = Helper.Lerp(1f, 1.5f, factor);
                        shadowAlpha = Helper.Lerp(1f, 0f, factor);

                        if (NPC.frame.Y != 0)
                        {
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
                    return false;
                case 3://短暂后摇
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 30)
                            return true;
                    }
                    return false;
            }
        }

    }
}
