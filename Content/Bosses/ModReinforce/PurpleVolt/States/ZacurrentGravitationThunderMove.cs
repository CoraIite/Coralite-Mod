using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 引力电球（连段件，不作单招出现）：<br/>
    /// Chase —— 边扇翅膀边贴近玩家，嘴前的紫尘从 240 px 收束到 100 px，这就是"要吐球了"的预告；
    /// 翅膀回到第 0 帧且过了 45 帧才进入下一拍，保证每次出手姿态一致。<br/>
    /// Shoot —— 抬头张嘴 10 帧后吐出一颗长时间牵引玩家的雷球。<br/>
    /// Burst / Recover —— 残影膨胀 40 帧（后坐表现），再 30 帧后摇收招。<br/>
    /// 旧 <c>ZacurrentDragon.GravitationThunder</c>（AI.GravitationThunder.cs:12-128）。
    /// </summary>
    internal static class ZacurrentGravitationThunderMove
    {
        private enum Beat
        {
            /// <summary>扇翅膀贴近（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>抬头吐球（旧 SonState 1）</summary>
            Shoot = 1,
            /// <summary>残影膨胀（旧 SonState 2）</summary>
            Burst = 2,
            /// <summary>后摇（旧 SonState 3）</summary>
            Recover = 3,
        }

        /// <summary>返回 true 表示整招结束。<paramref name="maxTime"/> 为引力雷球的持续时长。</summary>
        public static bool Run(ZacurrentDragonContext ctx, int maxTime = ZacurrentDirector.GravitationDefaultTime)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Shoot:
                    UpdateShoot(ctx, maxTime);
                    return false;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    return false;
                case Beat.Recover:
                    ctx.Boss.FlyingFrame();
                    ctx.Timer++;
                    return ctx.Timer > ZacurrentDirector.GravitationRecoverFrames;
                default:
                    UpdateChase(ctx);
                    return false;
            }
        }

        private static void UpdateChase(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;
            Vector2 pos = boss.GetMousePos();

            if (!VaultUtils.isServer)
            {
                // 嘴前的汇聚尘：散布半径随蓄力收束，收完即出手
                float edge = (ZacurrentDirector.GravitationGatherEdgeMax
                    - (ZacurrentDirector.GravitationGatherEdgeShrink * Math.Clamp(ctx.Timer / ZacurrentDirector.GravitationReadyFrames, 0, 1))) / 2;
                Vector2 center = pos + Helper.NextVec2Dir(edge - 1, edge);
                Dust dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                    (pos - center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(ZacurrentDirector.GravitationDustSpeedMin, ZacurrentDirector.GravitationDustSpeedMax),
                    newColor: ZacurrentDragon.ZacurrentDustPurple,
                    Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
                dust.noGravity = true;
            }

            npc.QuickSetDirection();
            boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out float yLength);

            if (xLength > ZacurrentDirector.GravitationChaseDeadZoneX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.GravitationChaseSpeedX,
                    ZacurrentDirector.GravitationChaseAccelX, ZacurrentDirector.GravitationChaseTurnX, ZacurrentDirector.GravitationChaseDampX);
            }
            else
            {
                npc.velocity.X *= ZacurrentDirector.GravitationChaseDampX;
            }

            if (npc.directionY < 0)
            {
                boss.FlyingUp(ZacurrentDirector.GravitationRiseAccel, ZacurrentDirector.GravitationRiseMax, ZacurrentDirector.GravitationRiseDamp);
            }
            else if (yLength > ZacurrentDirector.GravitationChaseDeadZoneY)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.GravitationChaseSpeedY,
                    ZacurrentDirector.GravitationChaseAccelY, ZacurrentDirector.GravitationChaseTurnY, ZacurrentDirector.GravitationChaseDampY);
                boss.FlyingFrame();
            }
            else
            {
                npc.velocity.Y *= ZacurrentDirector.GravitationChaseDampY;
                boss.FlyingFrame();
            }

            boss.SetRotationNormally();
            ctx.DeclareDirect();

            ctx.Timer++;
            if (npc.frame.Y == 0 && ctx.Timer > ZacurrentDirector.GravitationReadyFrames)
            {
                ctx.SonState = (int)Beat.Shoot;
                ctx.Timer = 0;
                ctx.MarkDecision();
            }
        }

        private static void UpdateShoot(ZacurrentDragonContext ctx, int maxTime)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity *= ZacurrentDirector.GravitationShootDamp;
            ctx.DeclareDirect();
            npc.QuickSetDirection();
            boss.TurnToNoRot();

            // 抬头到位才开始数出手帧
            if (npc.frame.Y != ZacurrentDirector.GravitationShootFrameY)
            {
                boss.FlyingFrame();
                return;
            }

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.GravitationShootDelay)
            {
                return;
            }

            ctx.SonState = (int)Beat.Burst;
            ctx.Timer = 0;
            ctx.MarkDecision();

            Vector2 mouth = boss.GetMousePos();
            if (!VaultUtils.isClient)
            {
                npc.NewProjectileDirectInAI<PurpleGravitationThunderBall>(mouth,
                    (ctx.Target.Center - mouth).SafeNormalize(Vector2.Zero) * ZacurrentDirector.GravitationBallSpeed,
                    ZacurrentDirector.GravitationBallDamage(), 0, npc.target, maxTime);
            }

            if (!VaultUtils.isServer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, npc.Center);
            }

            boss.canDrawShadows = true;
            boss.ResetAllOldCaches();
        }

        private static void UpdateBurst(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            float factor = Helper.SqrtEase(ctx.Timer / ZacurrentDirector.GravitationBurstFrames);
            boss.shadowScale = Helper.Lerp(1f, ZacurrentDirector.GravitationBurstShadowScale, factor);
            boss.shadowAlpha = Helper.Lerp(1f, 0f, factor);

            // 帧图快速走完回到 0（吐完球抖一下）
            if (npc.frame.Y != 0 && ++npc.frameCounter > ZacurrentDirector.GravitationBurstWingFrameTime)
            {
                npc.frameCounter = 0;
                if (++npc.frame.Y > ZacurrentDirector.WingFrameMax)
                {
                    npc.frame.Y = 0;
                }
            }

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.GravitationBurstFrames)
            {
                return;
            }

            boss.canDrawShadows = false;
            ctx.Timer = 0;
            ctx.SonState = (int)Beat.Recover;
            ctx.MarkDecision();
        }
    }
}
