using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>闪电突袭 / 电伏击穿共用的子拍号（旧 <c>ZacurrentDragon.LightningRaidState</c>，AI.LightningRaidNormal.cs:15-21）。</summary>
    internal enum ZacurrentRaidBeat
    {
        /// <summary>连续短冲</summary>
        SmallDash = 0,
        /// <summary>收翅后撤蓄力</summary>
        ReadyBigDash = 1,
        /// <summary>横穿全场的大冲</summary>
        BigDash = 2,
    }

    /// <summary>闪电突袭家族共用的小件：蓄力电粒子、蓄力期的距离带、大冲收尾。</summary>
    internal static class ZacurrentRaidShared
    {
        /// <summary>蓄力期的距离带：把跑道长度维持在 800~1000 px，太近退、太远追。旧 AI.LightningRaidNormal.cs:98-110</summary>
        public static void HoldRunway(ZacurrentDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            float distance = npc.Center.Distance(ctx.Target.Center);

            if (distance < ZacurrentDirector.RaidHoldNearDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.RaidHoldSpeedCap)
                {
                    npc.velocity += (npc.Center - ctx.Target.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.BallFlyAccel;
                }
            }
            else if (distance > ZacurrentDirector.RaidHoldFarDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.RaidHoldSpeedCap)
                {
                    npc.velocity += (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.BallFlyAccel;
                }
            }
            else
            {
                npc.velocity *= ZacurrentDirector.RaidHoldDamp;
            }

            ctx.DeclareDirect();
        }

        /// <summary>大冲起手的镜头冲击 + 风环 + 背景压暗（纯本地）。旧 AI.LightningRaidNormal.cs:162-170</summary>
        public static void DashLaunchEffects(ZacurrentDragonContext ctx, Vector2 dir, int dashFrames)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.RaidSkyLight, dashFrames - ZacurrentDirector.RaidSkyFadeLead, ZacurrentDirector.RaidSkyExchange);

            PunchCameraModifier modifier = new PunchCameraModifier(ctx.Npc.Center, dir * ZacurrentDirector.RaidPunchDirScale,
                ZacurrentDirector.RaidShakeStrength, ZacurrentDirector.RaidShakeVibration, ZacurrentDirector.RaidShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
            Main.instance.CameraModifiers.Add(modifier);

            WindCircle.Spawn(ctx.Npc.Center, -dir * ZacurrentDirector.ChainWindSpeed, dir.ToRotation(), ZacurrentDragon.ZacurrentPurple,
                ZacurrentDirector.ChainWindScale, ZacurrentDirector.ChainWindScaleMul, ZacurrentDirector.ChainWindStretch);
        }

        /// <summary>大冲结束时压平身体、熄灭电流环绕。旧 AI.LightningRaidNormal.cs:184-191</summary>
        public static void SettleAfterDash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.IsDashing = false;
            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
            npc.rotation = npc.direction > 0 ? 0 : ZacurrentDirector.FlatRotation;
            boss.currentSurrounding = false;
        }

        /// <summary>大冲收尾的刹车段。旧 AI.LightningRaidNormal.cs:192-201</summary>
        public static void BrakeAfterDash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity *= ZacurrentDirector.RaidBrakeDamp;
            ctx.DeclareDirect();
            boss.FlyingFrame();

            if (Math.Abs(npc.velocity.X) < ZacurrentDirector.RaidSettleSpeed)
            {
                npc.QuickSetDirection();
                npc.rotation = npc.direction > 0 ? 0 : ZacurrentDirector.FlatRotation;
            }
        }
    }

    /// <summary>
    /// 闪电突袭（普通形态单招，也是登场首招）：<br/>
    /// SmallDash —— 连续 2~4 次短冲拉近身位（远距离追加次数），每次都有起手锁向。<br/>
    /// ReadyBigDash —— 把翅膀从第 7 帧倒收回去，同时沿冲刺方向铺出一条 1000 px 的电粒子"跑道"——这就是大冲的预告，跑道指哪冲哪。<br/>
    /// BigDash —— 50 px/f 横穿，冲程 40%~80% 之间才追踪玩家（头尾不追），撞完刹停；
    /// <see cref="ZacurrentDragonContext.Recorder2"/> 还有余量就直接接下一轮，只歇 5 帧。<br/>
    /// 旧 <c>ZacurrentDragon.LightningRaidNoraml</c>（AI.LightningRaidNormal.cs:28-233）。
    /// </summary>
    internal static class ZacurrentLightningRaidNormalMove
    {
        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            switch ((ZacurrentRaidBeat)(int)ctx.SonState)
            {
                case ZacurrentRaidBeat.SmallDash:
                    UpdateSmallDash(ctx);
                    return false;
                case ZacurrentRaidBeat.ReadyBigDash:
                    UpdateReady(ctx);
                    return false;
                case ZacurrentRaidBeat.BigDash:
                    return UpdateBigDash(ctx);
                default:
                    return true;
            }
        }

        private static void UpdateSmallDash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (ctx.Timer == 0)
            {
                npc.NewProjectileInAI_Server<PurpleDash>(npc.Center, Vector2.Zero, ZacurrentDirector.SmallDashDamage(), 0
                    , npc.target, ZacurrentDirector.RaidSmallDashFrames - 1, npc.whoAmI, ZacurrentDirector.RaidSmallDashProjAi2);

                boss.ElectricSound();
                float targetRot = (ctx.Target.Center - npc.Center).ToRotation();
                if (Vector2.Distance(npc.Center, ctx.Target.Center) < ZacurrentDirector.SmallDashNoAimDistance)
                {
                    targetRot += ctx.AttackRandSign() * ctx.AttackRandFloat(ZacurrentDirector.SmallDashOffsetMin, ZacurrentDirector.SmallDashOffsetMax);
                }

                npc.velocity = targetRot.ToRotationVector2() * ZacurrentDirector.RaidSmallDashSpeed;
                npc.rotation = npc.velocity.ToRotation();
                npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                boss.IsDashing = true;
            }
            else if (ctx.Timer > ZacurrentDirector.RaidSmallDashFrames / ZacurrentDirector.SmallDashHomingStartDiv && ctx.Timer < ZacurrentDirector.RaidSmallDashFrames)
            {
                float distance = npc.Center.Distance(ctx.Target.Center);
                if (distance < ZacurrentDirector.SmallDashHomingDistance)
                {
                    float factor = 1 - Math.Clamp(distance / ZacurrentDirector.SmallDashHomingRange, 0.01f, 1);
                    float targetDir = (ctx.Target.Center - npc.Center).ToRotation() + MathHelper.Pi;
                    float velocityDir = npc.velocity.ToRotation().AngleTowards(targetDir, ZacurrentDirector.RaidSmallDashTurn * factor);
                    npc.velocity = velocityDir.ToRotationVector2() * ZacurrentDirector.RaidSmallDashSpeed;
                    npc.rotation = velocityDir;
                }
            }

            ctx.DeclareDirect();
            boss.UpdateAllOldCaches();

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.RaidSmallDashFrames)
            {
                return;
            }

            ctx.Timer = 0;
            ctx.Recorder--;
            if (ctx.Recorder < 1)
            {
                ctx.SonState = (int)ZacurrentRaidBeat.ReadyBigDash;
                ctx.MarkDecision();
            }

            npc.velocity *= 0;
            npc.frameCounter = 0;
            npc.frame.Y = ZacurrentDirector.WingFrameMax;
            boss.IsDashing = false;
        }

        private static void UpdateReady(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.QuickSetDirection();
            boss.SetRotationNormally(ZacurrentDirector.ChargeRotRate);
            ZacurrentRaidShared.HoldRunway(ctx);
            boss.UpdateAllOldCaches();

            // 收翅：帧图从 7 倒着走回 0
            if (++npc.frameCounter > ZacurrentDirector.RaidWingFrameTime)
            {
                npc.frameCounter = 0;
                npc.frame.Y--;
            }

            if (!VaultUtils.isServer)
            {
                if (ctx.Timer < ZacurrentDirector.RaidReadyFrames / 2)
                {
                    Vector2 pos = npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.width / 2);
                    pos += ctx.Timer / (ZacurrentDirector.RaidReadyFrames / 2) * (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.RaidParticleForward;
                    ZacurrentDragon.PurpleElectricParticle(pos);
                }

                if (Main.rand.NextBool())
                {
                    float radius = Helper.Lerp(ZacurrentDirector.RaidParticleRadiusMin, ZacurrentDirector.RaidParticleRadiusMax, ctx.Timer / ZacurrentDirector.RaidReadyFrames);
                    ZacurrentDragon.PurpleElectricParticle(npc.Center + Main.rand.NextVector2CircularEdge(radius, radius));
                }
            }

            ctx.Timer++;
            if (ctx.Timer > ZacurrentDirector.RaidReadyFrames || npc.frame.Y < 1)
            {
                ctx.SonState = (int)ZacurrentRaidBeat.BigDash;
                ctx.Timer = 0;
                ctx.MarkDecision();
            }
        }

        private static bool UpdateBigDash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            if (ctx.Timer == 0)
            {
                boss.IsDashing = true;
                npc.NewProjectileInAI_Server<PurpleDash>(npc.Center, Vector2.Zero, ZacurrentDirector.RaidBigDashDamage(), 0
                    , npc.target, ZacurrentDirector.RaidBigDashFrames, npc.whoAmI, ZacurrentDirector.RaidBigDashProjAi2);

                if (!VaultUtils.isServer)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
                }

                Vector2 dir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero);
                npc.velocity = dir * ZacurrentDirector.RaidBigDashSpeed;
                npc.rotation = npc.velocity.ToRotation();
                npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                ctx.DeclareDirect();
                ctx.MarkDecision();
                ZacurrentRaidShared.DashLaunchEffects(ctx, dir, ZacurrentDirector.RaidBigDashFrames);
            }
            else if (ctx.Timer > ZacurrentDirector.RaidBigDashFrames * ZacurrentDirector.RaidHomingStart
                && ctx.Timer < ZacurrentDirector.RaidBigDashFrames * ZacurrentDirector.RaidHomingEnd)
            {
                float factor = Math.Clamp(npc.Center.Distance(ctx.Target.Center) / ZacurrentDirector.RaidHomingRange, 0.01f, 1);
                float targetDir = (ctx.Target.Center - npc.Center).ToRotation();
                float velocityDir = npc.velocity.ToRotation().AngleTowards(targetDir, ZacurrentDirector.RaidBigDashTurn * factor);
                npc.velocity = velocityDir.ToRotationVector2() * ZacurrentDirector.RaidBigDashSpeed;
                npc.rotation = velocityDir;
                ctx.DeclareDirect();
            }

            if (ctx.Timer == ZacurrentDirector.RaidBigDashFrames)
            {
                ZacurrentRaidShared.SettleAfterDash(ctx);
            }
            else if (ctx.Timer > ZacurrentDirector.RaidBigDashFrames)
            {
                ZacurrentRaidShared.BrakeAfterDash(ctx);
            }

            ctx.Timer++;

            // 还有下一轮就几乎不歇，直接再冲
            int delay = ctx.Recorder2 > 0 ? ZacurrentDirector.RaidRestWhenChaining : ZacurrentDirector.RaidRestFrames();
            if (ctx.Timer <= ZacurrentDirector.RaidBigDashFrames + delay)
            {
                return false;
            }

            ctx.Timer = 0;
            ctx.Recorder2--;
            if (ctx.Recorder2 < 1)
            {
                return true;
            }

            // 新一轮：重置短冲次数
            ctx.Recorder = ZacurrentDirector.RaidNextRoundDashes;
            if (Vector2.Distance(npc.Center, ctx.Target.Center) > ZacurrentDirector.SmallDashExtraDistance1)
            {
                ctx.Recorder++;
            }

            ctx.SonState = (int)ZacurrentRaidBeat.SmallDash;
            ctx.MarkDecision();
            return false;
        }

        /// <summary>
        /// 进招初始值：短冲次数按距离加码，整招轮数最多 3 轮；登场首招固定 1 轮（从天而降那一下不该连冲）。
        /// 旧 AI.LightningRaidNormal.cs:243-263
        /// </summary>
        public static void SetStartValue(ZacurrentDragonContext ctx)
        {
            ctx.Recorder = ctx.AttackRandom.Next(ZacurrentDirector.RaidStartDashMin, ZacurrentDirector.RaidStartDashMax);

            float distance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);
            if (distance > ZacurrentDirector.SmallDashExtraDistance1)
            {
                ctx.Recorder++;
            }

            if (distance > ZacurrentDirector.SmallDashExtraDistance2)
            {
                ctx.Recorder++;
            }

            ctx.Recorder2 = ctx.ForceRecorder2OnNextLightningRaid ? 1 : ctx.AttackRandom.Next(ZacurrentDirector.RaidRoundMax);
            ctx.ForceRecorder2OnNextLightningRaid = false;

            ctx.Boss.ResetAllOldCaches();
            ctx.Boss.canDrawShadows = true;
        }
    }

    /// <summary>闪电突袭单招。旧壳 ZacurrentDragon.States.cs:175-180</summary>
    [VaultState((int)ZacurrentDragon.AIStates.LightningRaidNormal, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentLightningRaidNormalState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.LightningRaidNormal;

        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ZacurrentLightningRaidNormalMove.SetStartValue(ctx);

        protected override bool RunAttack(ZacurrentDragonContext ctx) => ZacurrentLightningRaidNormalMove.Run(ctx);
    }
}
