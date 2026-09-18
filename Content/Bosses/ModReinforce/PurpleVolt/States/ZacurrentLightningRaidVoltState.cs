using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 闪电突袭（紫伏形态单招）：与普通版同骨架，但短冲 7 帧 / 60 px/f、大冲 55 px/f 且带 Z 字折返，
    /// 蓄力期还多一层残影膨胀。两处关键差异：大冲是在<b>蓄力段结束的那一帧</b>就发动（起手更突然），
    /// 冲程中除了追踪还会在 4/20、7/20、13/20、16/20 四个折点各拐一次——这是紫伏形态"读不准落点"的来源。<br/>
    /// 旧 <c>ZacurrentDragon.LightningRaidVolt</c>（AI.LightningRaidVolt.cs:19-257）。
    /// </summary>
    internal static class ZacurrentLightningRaidVoltMove
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
                npc.NewProjectileInAI_Server<RedDash>(npc.Center, Vector2.Zero, ZacurrentDirector.SmallDashDamage(), 0
                    , npc.target, ZacurrentDirector.RaidVoltSmallDashFrames - 1, npc.whoAmI, ZacurrentDirector.RaidVoltSmallDashProjAi2);

                boss.ElectricSound();
                float targetRot = (ctx.Target.Center - npc.Center).ToRotation();
                if (Vector2.Distance(npc.Center, ctx.Target.Center) < ZacurrentDirector.SmallDashNoAimDistance)
                {
                    targetRot += ctx.AttackRandSign() * ctx.AttackRandFloat(ZacurrentDirector.SmallDashOffsetMin, ZacurrentDirector.SmallDashOffsetMax);
                }

                npc.velocity = targetRot.ToRotationVector2() * ZacurrentDirector.RaidVoltSmallDashSpeed;
                npc.rotation = npc.velocity.ToRotation();
                npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                boss.IsDashing = true;
            }
            else if (ctx.Timer > ZacurrentDirector.RaidVoltSmallDashFrames / ZacurrentDirector.SmallDashHomingStartDiv
                && ctx.Timer < ZacurrentDirector.RaidVoltSmallDashFrames)
            {
                float distance = npc.Center.Distance(ctx.Target.Center);
                if (distance < ZacurrentDirector.SmallDashHomingDistance)
                {
                    float factor = 1 - Math.Clamp(distance / ZacurrentDirector.SmallDashHomingRange, 0.01f, 1);
                    float targetDir = (ctx.Target.Center - npc.Center).ToRotation() + MathHelper.Pi;
                    float velocityDir = npc.velocity.ToRotation().AngleTowards(targetDir, ZacurrentDirector.RaidVoltSmallDashTurn * factor);
                    npc.velocity = velocityDir.ToRotationVector2() * ZacurrentDirector.RaidVoltSmallDashSpeed;
                    npc.rotation = velocityDir;
                }
            }

            ctx.DeclareDirect();
            boss.UpdateAllOldCaches();

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.RaidVoltSmallDashFrames)
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
                    ZacurrentDragon.RedElectricParticle(pos);
                }

                if (Main.rand.NextBool())
                {
                    float radius = Helper.Lerp(ZacurrentDirector.RaidParticleRadiusMin, ZacurrentDirector.RaidParticleRadiusMax, ctx.Timer / ZacurrentDirector.RaidReadyFrames);
                    ZacurrentDragon.RedElectricParticle(npc.Center + Main.rand.NextVector2CircularEdge(radius, radius));
                }
            }

            boss.shadowScale = Helper.Lerp(1, ZacurrentDirector.RaidShadowScaleMax, ctx.Timer / ZacurrentDirector.RaidReadyFrames);
            boss.shadowAlpha = Helper.Lerp(1, 0, ctx.Timer / ZacurrentDirector.RaidReadyFrames);

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.RaidReadyFrames && npc.frame.Y >= 1)
            {
                return;
            }

            // 紫伏版在蓄力段结束的这一帧直接发动大冲
            ctx.SonState = (int)ZacurrentRaidBeat.BigDash;
            ctx.Timer = 0;
            boss.IsDashing = true;

            npc.NewProjectileInAI_Server<RedDash>(npc.Center, Vector2.Zero, ZacurrentDirector.RaidVoltBigDashDamage(), 0
                , npc.target, ZacurrentDirector.RaidVoltBigDashFrames, npc.whoAmI, ZacurrentDirector.RaidVoltBigDashProjAi2);

            if (!VaultUtils.isServer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
            }

            Vector2 dir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            npc.velocity = dir * ZacurrentDirector.RaidVoltBigDashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
            boss.shadowScale = ZacurrentDirector.RaidDashShadowScale;
            boss.shadowAlpha = 1;
            ctx.DeclareDirect();
            ctx.MarkDecision();
            ZacurrentRaidShared.DashLaunchEffects(ctx, dir, ZacurrentDirector.RaidVoltBigDashFrames);
        }

        private static bool UpdateBigDash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            if (ctx.Timer > ZacurrentDirector.RaidVoltBigDashFrames * ZacurrentDirector.RaidHomingStart
                && ctx.Timer < ZacurrentDirector.RaidVoltBigDashFrames * ZacurrentDirector.RaidHomingEnd)
            {
                float factor = Math.Clamp(npc.Center.Distance(ctx.Target.Center) / ZacurrentDirector.RaidHomingRange, 0.01f, 1);
                float targetDir = (ctx.Target.Center - npc.Center).ToRotation();
                float velocityDir = npc.velocity.ToRotation().AngleTowards(targetDir, ZacurrentDirector.RaidVoltBigDashTurn * factor);
                npc.velocity = velocityDir.ToRotationVector2() * ZacurrentDirector.RaidVoltBigDashSpeed;
                npc.rotation = velocityDir;
                ctx.DeclareDirect();
            }

            if (ctx.Timer < ZacurrentDirector.RaidVoltBigDashFrames)
            {
                ZacurrentZMove.Apply(ctx, ZacurrentDirector.RaidVoltBigDashFrames);
                npc.rotation = npc.velocity.ToRotation();
                ctx.DeclareDirect();
            }
            else if (ctx.Timer == ZacurrentDirector.RaidVoltBigDashFrames)
            {
                ZacurrentRaidShared.SettleAfterDash(ctx);
            }
            else
            {
                ZacurrentRaidShared.BrakeAfterDash(ctx);
            }

            ctx.Timer++;

            int delay = ctx.Recorder2 > 0 ? ZacurrentDirector.RaidRestWhenChaining : ZacurrentDirector.RaidRestFrames();
            if (ctx.Timer <= ZacurrentDirector.RaidVoltBigDashFrames + delay)
            {
                return false;
            }

            ctx.Timer = 0;
            ctx.Recorder2--;
            if (ctx.Recorder2 < 1)
            {
                return true;
            }

            ctx.Recorder = ZacurrentDirector.RaidNextRoundDashes;
            if (Vector2.Distance(npc.Center, ctx.Target.Center) > ZacurrentDirector.SmallDashExtraDistance1)
            {
                ctx.Recorder++;
            }

            ctx.SonState = (int)ZacurrentRaidBeat.SmallDash;
            ctx.MarkDecision();
            return false;
        }
    }

    /// <summary>
    /// Z 字折返：在冲程的四个固定折点把速度各拐一次，左右左右地划过去。
    /// 拍点是确定性的（只看 Timer 与冲程长度），两端各算即可。旧 AI.VoltBreak.cs:136-158
    /// </summary>
    internal static class ZacurrentZMove
    {
        public static void Apply(ZacurrentDragonContext ctx, int dashFrames)
        {
            NPC npc = ctx.Npc;

            if (ctx.Timer == dashFrames * ZacurrentDirector.ZMoveStep1 / ZacurrentDirector.ZMoveDenominator)
            {
                npc.velocity = npc.velocity.RotatedBy(ZacurrentDirector.ZMoveAngle);
            }

            if (ctx.Timer == dashFrames * ZacurrentDirector.ZMoveStep2 / ZacurrentDirector.ZMoveDenominator)
            {
                npc.velocity = npc.velocity.RotatedBy(-ZacurrentDirector.ZMoveAngle * 2);
            }

            if (ctx.Timer == dashFrames * ZacurrentDirector.ZMoveStep3 / ZacurrentDirector.ZMoveDenominator)
            {
                npc.velocity = npc.velocity.RotatedBy(ZacurrentDirector.ZMoveAngle * 2);
            }

            if (ctx.Timer == dashFrames * ZacurrentDirector.ZMoveStep4 / ZacurrentDirector.ZMoveDenominator)
            {
                npc.velocity = npc.velocity.RotatedBy(-ZacurrentDirector.ZMoveAngle);
            }
        }
    }

    /// <summary>紫伏闪电突袭单招。旧壳 ZacurrentDragon.States.cs:182-187</summary>
    [VaultState((int)ZacurrentDragon.AIStates.LightningRaidVolt, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentLightningRaidVoltState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.LightningRaidVolt;

        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ZacurrentLightningRaidNormalMove.SetStartValue(ctx);

        protected override bool RunAttack(ZacurrentDragonContext ctx) => ZacurrentLightningRaidVoltMove.Run(ctx);
    }
}
