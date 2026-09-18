using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 冲刺放电（远距离单招）：<br/>
    /// Retreat —— 扇一轮翅膀往后拉开，翅膀扇完就以 50 px/f 扑过来。<br/>
    /// Dash —— 一路追到玩家 200 px 内（或 60 帧上限）急刹。<br/>
    /// Charge —— 原地充电 48 帧，一圈电尘从 150 px 张到 540 px，抬头到位——这是全招最长的预告，贴脸的玩家有整整 0.8 秒撤。<br/>
    /// Burst —— 60 帧放电，期间前 2/3 还能以 0.01 rad/f 微调朝向，结束时朝五个等分方向各射一道电球雷。<br/>
    /// Recover —— 25 帧后摇。<br/>
    /// 旧 <c>ZacurrentDragon.DashDischarging</c>（AI.DashDischarging.cs:14-213）。
    /// </summary>
    internal static class ZacurrentDashDischargingMove
    {
        private enum Beat
        {
            /// <summary>扇翅膀拉开距离（旧 SonState 0）</summary>
            Retreat = 0,
            /// <summary>扑向玩家（旧 1）</summary>
            Dash = 1,
            /// <summary>原地充电（旧 2）</summary>
            Charge = 2,
            /// <summary>放电（旧 3）</summary>
            Burst = 3,
            /// <summary>后摇（旧 4）</summary>
            Recover = 4,
        }

        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Dash:
                    UpdateDash(ctx);
                    return false;
                case Beat.Charge:
                    UpdateCharge(ctx);
                    return false;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    return false;
                case Beat.Recover:
                    ctx.Boss.FlyingFrame();
                    ctx.Timer++;
                    return ctx.Timer > ZacurrentDirector.DischargeRecoverFrames;
                default:
                    UpdateRetreat(ctx);
                    return false;
            }
        }

        private static void UpdateRetreat(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (ctx.Timer == 0)
            {
                npc.frame.Y = 1;
                ctx.Timer = 1;
            }

            boss.SetSpriteDirectionFoTarget();
            boss.SetRotationNormally(ZacurrentDirector.ChargeRotRate);

            if (npc.velocity.Length() < ZacurrentDirector.ChainRetreatSpeedCap)
            {
                npc.velocity += (npc.Center - ctx.Target.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ChainRetreatAccel;
            }

            ctx.DeclareDirect();

            if (++npc.frameCounter <= ZacurrentDirector.WingFrameTime)
            {
                return;
            }

            npc.frameCounter = 0;
            npc.frame.Y++;
            if (npc.frame.Y <= ZacurrentDirector.WingFrameMax)
            {
                return;
            }

            ctx.SonState = (int)Beat.Dash;
            ctx.Timer = 0;
            boss.ResetAllOldCaches();
            boss.canDrawShadows = true;
            boss.shadowScale = ZacurrentDirector.ChainShadowScale;
            boss.IsDashing = true;
            ctx.MarkDecision();

            Vector2 dir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            npc.velocity = dir * ZacurrentDirector.ChainDashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched("Electric/ElectricShoot", 0.4f, 0, npc.Center);
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
                WindCircle.Spawn(npc.Center, -dir * ZacurrentDirector.ChainWindSpeed, dir.ToRotation(), ZacurrentDragon.ZacurrentPurple,
                    ZacurrentDirector.ChainWindScale, ZacurrentDirector.ChainWindScaleMul, ZacurrentDirector.ChainWindStretch);
            }
        }

        private static void UpdateDash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();
            boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out _);
            if (xLength > ZacurrentDirector.DischargeFaceDeadZoneX)
            {
                npc.QuickSetDirection();
            }

            // 全程直追，所以它是"躲不掉的接近"，代价是后面的充电很长
            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ChainDashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            ctx.DeclareDirect();

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.DischargeDashFrames
                && Vector2.Distance(ctx.Target.Center, npc.Center) >= ZacurrentDirector.DischargeArriveDistance)
            {
                return;
            }

            ctx.SonState = (int)Beat.Charge;
            ctx.Timer = 0;
            npc.velocity *= ZacurrentDirector.DischargeArriveDamp;
            npc.frame.Y = 0;
            npc.frameCounter = 0;
            boss.IsDashing = false;
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

            boss.UpdateAllOldCaches();
            boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out float yLength);
            npc.QuickSetDirection();
            boss.TurnToNoRot(ZacurrentDirector.ChargeRotRate);

            if (xLength > ZacurrentDirector.DischargeFaceDeadZoneX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.DischargeHoldSpeedX,
                    ZacurrentDirector.DischargeHoldAccelX, ZacurrentDirector.DischargeHoldTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.X *= ZacurrentDirector.BallChaseDamp;
            }

            if (yLength > ZacurrentDirector.BallChaseDeadZoneY)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.DischargeHoldSpeedY,
                    ZacurrentDirector.DischargeHoldAccelY, ZacurrentDirector.DischargeHoldTurnY, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.Y *= ZacurrentDirector.BallChaseDamp;
            }

            ctx.DeclareDirect();

            ctx.Timer++;
            float edge = (ZacurrentDirector.DischargeRingMin
                + ((ZacurrentDirector.DischargeRingMax - ZacurrentDirector.DischargeRingMin) * Math.Clamp(ctx.Timer / ZacurrentDirector.DischargeChargeFrames, 0, 1))) / 2;
            for (int i = 0; i < ZacurrentDirector.DischargeDustPerFrame; i++)
            {
                SpawnChargeDust(ctx, edge);
            }

            // 抬头到位才开始数出手帧
            if (npc.frame.Y != ZacurrentDirector.BallSwingFrameY)
            {
                if (++npc.frameCounter > ZacurrentDirector.DischargeWingFrameTime)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y++;
                }

                return;
            }

            if (ctx.Timer <= ZacurrentDirector.DischargeChargeFrames)
            {
                return;
            }

            ctx.SonState = (int)Beat.Burst;
            ctx.Timer = 0;
            ctx.Recorder = (ctx.Target.Center - npc.Center).ToRotation();
            npc.TargetClosest();
            ctx.MarkDecision();

            if (!VaultUtils.isClient)
            {
                npc.NewProjectileDirectInAI<PurpleDischargingBurst>(npc.Center, Vector2.Zero, ZacurrentDirector.DischargeBurstDamage(), 0, npc.target
                    , ZacurrentDirector.DischargeBurstFrames, npc.whoAmI);
            }

            if (!VaultUtils.isServer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, npc.Center);
                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, npc.Center);

                PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, Vector2.UnitY * ZacurrentDirector.DischargePunchY,
                    ZacurrentDirector.DischargeShakeStrength, ZacurrentDirector.DischargeShakeVibration, ZacurrentDirector.DischargeShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
                Main.instance.CameraModifiers.Add(modifier);

                ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.DischargeSkyLight, ZacurrentDirector.BallSkySeadeFrames, ZacurrentDirector.BallSkyExchange);
            }

            boss.OpenMouse = true;
            boss.canDrawShadows = true;
            boss.currentSurrounding = true;
            boss.ResetAllOldCaches();
        }

        private static void UpdateBurst(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();
            boss.TurnToNoRot(ZacurrentDirector.ChargeRotRate);

            float factor = Helper.SqrtEase(ctx.Timer / ZacurrentDirector.DischargeBurstFrames);
            boss.shadowScale = Helper.Lerp(1f, ZacurrentDirector.DischargeShadowScaleMax, factor * ZacurrentDirector.DischargeShadowCycles % 1);
            boss.shadowAlpha = Helper.Lerp(1f, 0f, factor);

            npc.velocity *= ZacurrentDirector.DischargeBurstDamp;
            ctx.DeclareDirect();

            if (npc.frame.Y != 0 && ++npc.frameCounter > ZacurrentDirector.DischargeBurstWingFrameTime)
            {
                npc.frameCounter = 0;
                if (++npc.frame.Y > ZacurrentDirector.WingFrameMax)
                {
                    npc.frame.Y = 0;
                }
            }

            // 前 2/3 还能慢慢转，之后角度锁死 = 预警臂指哪打哪
            if (ctx.Timer < ZacurrentDirector.DischargeBurstFrames * ZacurrentDirector.DischargeTrackNumerator / ZacurrentDirector.DischargeTrackDenominator)
            {
                ctx.Recorder = ctx.Recorder.AngleTowards((ctx.Target.Center - npc.Center).ToRotation(), ZacurrentDirector.DischargeTrackRate);
            }

            SpawnArmDust(ctx, ctx.Timer / ZacurrentDirector.DischargeBurstFrames);

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.DischargeBurstFrames)
            {
                return;
            }

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched("Electric/ElectricShoot", 0.4f, 0f, npc.Center);
            }

            if (!VaultUtils.isClient)
            {
                int damage = ZacurrentDirector.DischargeArmDamage();
                for (int i = 0; i < ZacurrentDirector.DischargeArmCount; i++)
                {
                    Vector2 dir = (ctx.Recorder + (i * MathHelper.TwoPi / ZacurrentDirector.DischargeArmCount)).ToRotationVector2();
                    Vector2 pos = npc.Center + (dir * ZacurrentDirector.DischargeArmRoot);
                    npc.NewProjectileInAI<PurpleElectricBallThunder>(pos + (dir * ZacurrentDirector.DischargeArmReach), pos, damage, 0, npc.target
                        , ZacurrentDirector.DischargeArmProjAi0, npc.whoAmI, ZacurrentDirector.DischargeArmProjAi2);
                }
            }

            boss.canDrawShadows = false;
            boss.currentSurrounding = false;
            ctx.Timer = 0;
            ctx.SonState = (int)Beat.Recover;
            ctx.MarkDecision();
        }

        /// <summary>充电环的尘：一圈沿切向甩、一圈朝外散。纯本地。旧 AI.DashDischarging.cs:216-231</summary>
        private static void SpawnChargeDust(ZacurrentDragonContext ctx, float edge)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            NPC npc = ctx.Npc;
            Vector2 pos = npc.Center + Main.rand.NextVector2CircularEdge(edge, edge);
            Dust dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail
                , (pos - npc.Center).SafeNormalize(Vector2.Zero).RotatedBy(npc.direction * MathHelper.PiOver2) * Main.rand.NextFloat(ZacurrentDirector.DischargeDustSpeedMin, ZacurrentDirector.DischargeDustSpeedMax)
                , newColor: ZacurrentDragon.ZacurrentDustPurple
                , Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
            dust.noGravity = true;

            pos = npc.Center + Main.rand.NextVector2Circular(edge, edge);
            dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail
                , (pos - npc.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(ZacurrentDirector.DischargeDustSpeedMin, ZacurrentDirector.DischargeDustSpeedMax)
                , newColor: ZacurrentDragon.ZacurrentPink);
            dust.noGravity = true;
        }

        /// <summary>五条放电臂的预警尘：臂长随放电进度伸长，指示最后五道电球雷的方向。纯本地。旧 AI.DashDischarging.cs:233-244</summary>
        private static void SpawnArmDust(ZacurrentDragonContext ctx, float factor)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            NPC npc = ctx.Npc;
            float length = Helper.Lerp(ZacurrentDirector.DischargeArmLength, ZacurrentDirector.DischargeArmLength + ZacurrentDirector.DischargeArmGrow, factor);

            for (int i = 0; i < ZacurrentDirector.DischargeArmCount; i++)
            {
                Vector2 dir = (ctx.Recorder + (i * MathHelper.TwoPi / ZacurrentDirector.DischargeArmCount)).ToRotationVector2();
                Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(6, 6) + (dir * Main.rand.NextFloat(20, length)), DustID.PortalBoltTrail
                    , dir.RotateByRandom(-ZacurrentDirector.CannonDustJitter, ZacurrentDirector.CannonDustJitter) * Main.rand.NextFloat(ZacurrentDirector.GravitationDustSpeedMin, ZacurrentDirector.CannonDustSpeedMax)
                    , newColor: ZacurrentDragon.ZacurrentDustPurple
                    , Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
                dust.noGravity = true;
            }
        }
    }

    /// <summary>冲刺放电单招。旧壳 ZacurrentDragon.States.cs:224-228</summary>
    [VaultState((int)ZacurrentDragon.AIStates.DashDischarging, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentDashDischargingState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.DashDischarging;

        protected override bool RunAttack(ZacurrentDragonContext ctx) => ZacurrentDashDischargingMove.Run(ctx);
    }
}
