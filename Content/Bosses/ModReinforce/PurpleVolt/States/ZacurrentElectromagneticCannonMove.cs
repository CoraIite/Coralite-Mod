using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 电磁炮（连段件，本 boss 唯一的持续射线）：<br/>
    /// Chase —— 高速飞到玩家上方 300 px（最多 4 秒）。<br/>
    /// Roll —— 垂直切出去绕 45 帧，摆出侧身姿态。<br/>
    /// Aim —— 前 10 帧还跟着玩家转，之后角度锁死并沿射线方向铺满 1220 px 的预警尘，抬头到位后 35 帧开火。<br/>
    /// Burst —— 2.5 秒射线，只以 0.017 rad/f（约每秒 58°）扫动，这个慢扫速就是逃生道：绕着龙跑就能出线。<br/>
    /// Recover —— 25 帧后摇。<br/>
    /// 旧 <c>ZacurrentDragon.ElectromagneticCannon</c>（AI.ElectromagneticCannon.cs:11-188）。
    /// </summary>
    internal static class ZacurrentElectromagneticCannonMove
    {
        private enum Beat
        {
            /// <summary>飞到玩家上方（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>绕飞半圈（旧 SonState 1）</summary>
            Roll = 1,
            /// <summary>锁定瞄准（旧 SonState 2）</summary>
            Aim = 2,
            /// <summary>射线持续（旧 SonState 3）</summary>
            Burst = 3,
            /// <summary>后摇（旧 SonState 4）</summary>
            Recover = 4,
        }

        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Roll:
                    UpdateRoll(ctx);
                    return false;
                case Beat.Aim:
                    UpdateAim(ctx);
                    return false;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    return false;
                case Beat.Recover:
                    ctx.Boss.TurnToNoRot();
                    ctx.Boss.FlyingFrame();
                    return ++ctx.Timer > ZacurrentDirector.CannonRecoverFrames;
                default:
                    UpdateChase(ctx);
                    return false;
            }
        }

        private static void UpdateChase(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;
            Vector2 targetPos = ctx.Target.Center + new Vector2(0, -ZacurrentDirector.CannonHoverAbove);

            npc.direction = npc.spriteDirection = targetPos.X > npc.Center.X ? 1 : -1;
            npc.directionY = targetPos.Y > npc.Center.Y ? 1 : -1;
            boss.SetRotationNormally();
            boss.GetLengthToTargetPos(targetPos, out float xLength, out float yLength);

            if (xLength > ZacurrentDirector.CannonChaseDeadZoneX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.CannonChaseSpeedX,
                    ZacurrentDirector.CannonChaseAccelX, ZacurrentDirector.CannonChaseTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.X *= ZacurrentDirector.BallChaseDamp;
            }

            if (npc.directionY < 0)
            {
                boss.FlyingUp(ZacurrentDirector.CannonRiseAccel, ZacurrentDirector.CannonRiseMax, ZacurrentDirector.GravitationRiseDamp);
            }
            else if (yLength > ZacurrentDirector.BallChaseDeadZoneY)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.CannonChaseSpeedY,
                    ZacurrentDirector.CannonChaseAccelY, ZacurrentDirector.CannonChaseTurnY, ZacurrentDirector.BallChaseDamp);
                boss.FlyingFrame();
            }
            else
            {
                npc.velocity.Y *= ZacurrentDirector.BallChaseDamp;
                // 高度已经对上，加速把就位段走完
                ctx.Timer += ZacurrentDirector.CannonCloseTimeBonus;
                boss.FlyingFrame();
            }

            ctx.DeclareDirect();

            ctx.Timer++;
            bool arrived = xLength < ZacurrentDirector.CannonChaseDeadZoneX && yLength < ZacurrentDirector.CannonArriveY;
            if (ctx.Timer <= ZacurrentDirector.CannonChaseTimeout && !arrived)
            {
                return;
            }

            ctx.SonState = (int)Beat.Roll;
            ctx.Timer = 0;
            boss.ResetAllOldCaches();
            boss.canDrawShadows = true;
            boss.IsDashing = true;
            boss.shadowScale = ZacurrentDirector.CannonRollShadowScale;
            boss.shadowAlpha = 1;
            boss.currentSurrounding = true;
            ctx.MarkDecision();

            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero)
                .RotatedBy(-npc.direction * MathHelper.PiOver2) * ZacurrentDirector.CannonRollLaunchSpeed;
        }

        private static void UpdateRoll(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();
            npc.velocity = npc.velocity.RotatedBy(-npc.spriteDirection * MathHelper.TwoPi / ZacurrentDirector.CannonRollDivisor);
            npc.rotation = npc.velocity.ToRotation();
            ctx.DeclareDirect();

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.CannonRollFrames)
            {
                return;
            }

            ctx.SonState = (int)Beat.Aim;
            ctx.Timer = 0;
            boss.TurnToNoRot();
            boss.IsDashing = false;
            ctx.MarkDecision();
        }

        private static void UpdateAim(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity *= ZacurrentDirector.CannonAimDamp;
            ctx.DeclareDirect();
            boss.UpdateAllOldCaches();

            // 前 10 帧还跟手，之后 Recorder 里的角度就锁死了
            if (ctx.Timer < ZacurrentDirector.CannonAimLockFrames)
            {
                npc.QuickSetDirection();
                boss.TurnToNoRot();
                ctx.Recorder = (ctx.Target.Center - npc.Center).ToRotation();
            }

            if (!VaultUtils.isServer)
            {
                Vector2 muzzle = npc.Center + ((npc.rotation - (npc.direction * ZacurrentDirector.CannonMuzzleAngleOffset)).ToRotationVector2() * ZacurrentDirector.CannonMuzzleDistance);
                Vector2 dir = ctx.Recorder.ToRotationVector2();
                for (int i = 0; i < ZacurrentDirector.CannonDustPerFrame; i++)
                {
                    Dust dust = Dust.NewDustPerfect(muzzle + (dir * Main.rand.NextFloat(ZacurrentDirector.CannonDustRangeMin, ZacurrentDirector.CannonDustRangeMax)),
                        DustID.PortalBoltTrail,
                        dir.RotateByRandom(-ZacurrentDirector.CannonDustJitter, ZacurrentDirector.CannonDustJitter) * Main.rand.NextFloat(ZacurrentDirector.CannonDustSpeedMin, ZacurrentDirector.CannonDustSpeedMax),
                        newColor: ZacurrentDragon.ZacurrentDustPurple,
                        Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
                    dust.noGravity = true;
                }
            }

            if (npc.frame.Y != ZacurrentDirector.BallSwingFrameY)
            {
                boss.FlyingFrame();
                return;
            }

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.CannonAimFrames)
            {
                return;
            }

            ctx.SonState = (int)Beat.Burst;
            ctx.Timer = 0;
            npc.velocity *= 0;
            npc.TargetClosest();
            ctx.MarkDecision();

            Vector2 mouth = boss.GetMousePos();
            if (!VaultUtils.isClient)
            {
                npc.NewProjectileDirectInAI<PurpleElectromagneticCannon>(mouth + (ctx.Recorder.ToRotationVector2() * ZacurrentDirector.CannonSpawnLead), mouth,
                    ZacurrentDirector.CannonDamage(), 0, npc.target, ZacurrentDirector.CannonBurstFrames, npc.whoAmI, ZacurrentDirector.CannonProjAi2);
            }

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched(CoraliteSoundID.PhantasmalDeathray_Zombie104, npc.Center, pitch: 0.3f);

                PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, ctx.Recorder.ToRotationVector2() * ZacurrentDirector.CannonPunchDirScale,
                    ZacurrentDirector.CannonShakeStrength, ZacurrentDirector.CannonShakeVibration, ZacurrentDirector.CannonShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
                Main.instance.CameraModifiers.Add(modifier);

                ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.CannonSkyLight,
                    ZacurrentDirector.CannonBurstFrames * ZacurrentDirector.CannonSkyFadeNumerator / ZacurrentDirector.CannonSkyFadeDenominator,
                    ZacurrentDirector.CannonSkyExchange);
            }

            boss.canDrawShadows = true;
            boss.ResetAllOldCaches();
        }

        private static void UpdateBurst(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            // 朝向取射线远端，死区放宽，防止射线扫过头顶时来回翻面
            boss.SetSpriteDirectionFoTarget(boss.GetMousePos() + (ctx.Recorder.ToRotationVector2() * ZacurrentDirector.CannonFaceLead), ZacurrentDirector.CannonFaceDeadZone);
            boss.TurnToNoRot(1);
            ctx.Recorder = ctx.Recorder.AngleTowards((ctx.Target.Center - boss.GetMousePos()).ToRotation(), ZacurrentDirector.CannonTrackRate);
            boss.FlyingFrame();

            float factor = Helper.SqrtEase(ctx.Timer / ZacurrentDirector.CannonBurstFrames);
            boss.shadowScale = Helper.Lerp(1f, ZacurrentDirector.CannonBurstShadowScale, factor);
            boss.shadowAlpha = Helper.Lerp(1f, 0f, factor);

            ctx.Timer++;
            if (!VaultUtils.isServer && ctx.Timer > 0 && ctx.Timer % ZacurrentDirector.CannonBurstShakeInterval == 0)
            {
                PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, Helper.NextVec2Dir(),
                    ZacurrentDirector.CannonBurstShakeStrength, ZacurrentDirector.CannonBurstShakeVibration, ZacurrentDirector.CannonShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
                Main.instance.CameraModifiers.Add(modifier);
            }

            if (ctx.Timer <= ZacurrentDirector.CannonBurstFrames)
            {
                return;
            }

            boss.canDrawShadows = false;
            boss.currentSurrounding = false;
            ctx.Timer = 0;
            ctx.SonState = (int)Beat.Recover;
            npc.QuickSetDirection();
            boss.TurnToNoRot();
            ctx.MarkDecision();
        }
    }
}
