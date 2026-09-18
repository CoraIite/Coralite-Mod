using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 电伏击穿（紫伏连段件）：<br/>
    /// Approach —— 绕到玩家左边或右边 700 px（方向由 <see cref="ZacurrentDragonContext.Recorder"/> 锁死），到位或 3 秒超时都进下一拍。<br/>
    /// Charge —— 36 帧收翅蓄力，红电沿横向铺出 1000 px 跑道、残影膨胀到 2.5——预告的是一条<b>水平</b>轨迹，
    /// 所以躲法是上下走，不是左右跑。<br/>
    /// Dash —— 55 px/f 横穿全场，途中 Z 字折返四次，每 3 帧向自己脚下 400 px 丢一发落雷（横穿轨迹下方会留下一排雷）。<br/>
    /// 旧 <c>ZacurrentDragon.VoltBreak</c>（AI.VoltBreak.cs:13-197）。
    /// </summary>
    internal static class ZacurrentVoltBreakMove
    {
        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            switch ((ZacurrentRaidBeat)(int)ctx.SonState)
            {
                case ZacurrentRaidBeat.ReadyBigDash:
                    UpdateCharge(ctx);
                    return false;
                case ZacurrentRaidBeat.BigDash:
                    return UpdateDash(ctx);
                default:
                    UpdateApproach(ctx);
                    return false;
            }
        }

        private static void UpdateApproach(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            Vector2 targetPos = ctx.Target.Center + new Vector2(ctx.Recorder * ZacurrentDirector.BreakSideDistance, 0);
            Vector2 dir = targetPos - npc.Center;

            float speed = npc.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / ZacurrentDirector.BreakApproachRange, 0, 1) * ZacurrentDirector.BreakApproachMaxSpeed;
            npc.velocity = dir.ToRotation().ToRotationVector2() * Helper.Lerp(speed, aimSpeed, ZacurrentDirector.BreakApproachLerp);
            ctx.DeclareDirect();

            boss.SetSpriteDirectionFoTarget(targetPos);
            boss.SetRotationNormally();
            boss.FlyingFrame();

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.BreakApproachTimeout
                && Vector2.Distance(targetPos, npc.Center) >= ZacurrentDirector.BreakArriveDistance)
            {
                return;
            }

            ctx.SonState = (int)ZacurrentRaidBeat.ReadyBigDash;
            ctx.Timer = 0;
            npc.frame.Y = ZacurrentDirector.BreakChargeStartFrameY;
            ctx.MarkDecision();

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched("Electric/Charge", 1f, 0, npc.Center);
            }
        }

        private static void UpdateCharge(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.SetSpriteDirectionFoTarget();
            boss.TurnToNoRot();

            // 前半段还允许微调身位，后半段锁死（起手位置即承诺）
            if (ctx.Timer < ZacurrentDirector.BreakChargeFrames / 2)
            {
                Vector2 targetPos = ctx.Target.Center + new Vector2(ctx.Recorder * ZacurrentDirector.BreakSideHoldDistance, 0);
                boss.GetLengthToTargetPos(targetPos, out float xLength, out float yLength);

                if (xLength > ZacurrentDirector.BallChaseDeadZoneX)
                {
                    Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.BreakHoldSpeedX,
                        ZacurrentDirector.BreakHoldAccelX, ZacurrentDirector.BreakHoldTurnX, ZacurrentDirector.BallChaseDamp);
                }
                else
                {
                    npc.velocity.X *= ZacurrentDirector.BallChaseDamp;
                }

                if (npc.directionY < 0)
                {
                    boss.FlyingUp(ZacurrentDirector.GravitationRiseAccel, ZacurrentDirector.GravitationRiseMax, ZacurrentDirector.GravitationRiseDamp);
                }
                else if (yLength > ZacurrentDirector.BallChaseDeadZoneY)
                {
                    Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.BreakHoldSpeedY,
                        ZacurrentDirector.BreakHoldAccelY, ZacurrentDirector.BreakHoldTurnY, ZacurrentDirector.BallChaseDamp);
                }
                else
                {
                    npc.velocity.Y *= ZacurrentDirector.GravitationRiseDamp;
                }
            }

            ctx.DeclareDirect();
            boss.UpdateAllOldCaches();

            if (++npc.frameCounter > ZacurrentDirector.RaidWingFrameTime)
            {
                npc.frameCounter = 0;
                npc.frame.Y--;
            }

            if (!VaultUtils.isServer)
            {
                if (ctx.Timer < ZacurrentDirector.BreakChargeFrames / 2)
                {
                    // 横向铺出跑道：预告的是水平轨迹
                    Vector2 pos = npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.width / 2);
                    pos += ctx.Timer / (ZacurrentDirector.BreakChargeFrames / 2) * new Vector2(-ctx.Recorder, 0) * ZacurrentDirector.RaidParticleForward;
                    ZacurrentDragon.RedElectricParticle(pos);
                }

                if (Main.rand.NextBool())
                {
                    float radius = Helper.Lerp(ZacurrentDirector.RaidParticleRadiusMin, ZacurrentDirector.RaidParticleRadiusMax, ctx.Timer / ZacurrentDirector.BreakChargeFrames);
                    ZacurrentDragon.RedElectricParticle(npc.Center + Main.rand.NextVector2CircularEdge(radius, radius));
                }
            }

            boss.shadowScale = Helper.Lerp(1, ZacurrentDirector.RaidShadowScaleMax, ctx.Timer / ZacurrentDirector.BreakChargeFrames);
            boss.shadowAlpha = Helper.Lerp(1, 0, ctx.Timer / ZacurrentDirector.BreakChargeFrames);

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.BreakChargeFrames)
            {
                return;
            }

            ctx.SonState = (int)ZacurrentRaidBeat.BigDash;
            ctx.Timer = 0;
            boss.IsDashing = true;

            npc.NewProjectileInAI_Server<RedDash>(npc.Center, Vector2.Zero, ZacurrentDirector.BreakDashDamage(), 0
                , npc.target, ZacurrentDirector.BreakDashFrames, npc.whoAmI, ZacurrentDirector.BreakDashProjAi2);

            if (!VaultUtils.isServer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
            }

            // 冲刺方向是纯水平的，与蓄力期铺出的跑道一致
            Vector2 dashDir = new Vector2(-ctx.Recorder, 0);
            npc.velocity = dashDir * ZacurrentDirector.BreakDashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
            boss.shadowScale = ZacurrentDirector.RaidDashShadowScale;
            boss.shadowAlpha = 1;
            ctx.DeclareDirect();
            ctx.MarkDecision();

            // 镜头与风环按"朝玩家"的方向踢（旧行为，与实际冲刺方向可以不同）
            ZacurrentRaidShared.DashLaunchEffects(ctx, (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero), ZacurrentDirector.BreakDashFrames);
        }

        private static bool UpdateDash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            if (ctx.Timer < ZacurrentDirector.BreakDashFrames)
            {
                ZacurrentZMove.Apply(ctx, ZacurrentDirector.BreakDashFrames);
                npc.rotation = npc.velocity.ToRotation();
                ctx.DeclareDirect();

                if (ctx.Timer % ZacurrentDirector.BreakThunderInterval == 0)
                {
                    npc.NewProjectileInAI_Server<PurpleSmallThunderFall>(npc.Center + new Vector2(0, ZacurrentDirector.BreakThunderDropY), Vector2.Zero
                        , ZacurrentDirector.BreakThunderDamage(), 0, npc.target
                        , ZacurrentDirector.BreakThunderDelayBase + ctx.Timer, npc.whoAmI, ZacurrentDirector.BreakThunderAi2);
                }
            }
            else if (ctx.Timer == ZacurrentDirector.BreakDashFrames)
            {
                ZacurrentRaidShared.SettleAfterDash(ctx);
            }
            else
            {
                ZacurrentRaidShared.BrakeAfterDash(ctx);
            }

            ctx.Timer++;
            return ctx.Timer > ZacurrentDirector.BreakDashFrames + ZacurrentDirector.RaidRestFrames();
        }

        /// <summary>进招初始值：掷出从左边还是右边横穿。旧 AI.VoltBreak.cs:200-203</summary>
        public static void SetStartValue(ZacurrentDragonContext ctx)
            => ctx.Recorder = ctx.AttackRandom.Next(2) == 0 ? -1 : 1;
    }
}
