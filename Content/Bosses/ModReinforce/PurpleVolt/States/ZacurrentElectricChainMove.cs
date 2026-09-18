using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 闪电链（连段件，不作单招出现）：<br/>
    /// Retreat —— 扇一轮翅膀同时向后拉开距离，翅膀走完一圈即出手，退位本身就是预告。<br/>
    /// Roll —— 瞄玩家外侧 500 px（不直冲玩家）以 50 px/f 切进场，随后沿玩家周围 600 px 绕 1.5 圈，
    /// 每 3 帧在身后留一段电链，后留的链寿命更长，绕完一圈时整张网同时还在场上。<br/>
    /// Recover —— 按距离带把身位调回中距（太近继续退、太远追上来），20 帧后收招。<br/>
    /// 旧 <c>ZacurrentDragon.ElectricChain</c>（AI.ElectricChain.cs:13-132）。
    /// </summary>
    internal static class ZacurrentElectricChainMove
    {
        private enum Beat
        {
            /// <summary>扇翅膀 + 拉开距离（旧 SonState 0）</summary>
            Retreat = 0,
            /// <summary>切入 + 绕飞布链（旧 SonState 1）</summary>
            Roll = 1,
            /// <summary>后摇归位（旧 SonState 2）</summary>
            Recover = 2,
        }

        /// <summary>返回 true 表示整招结束。<paramref name="chainTime"/> 为每段电链的基础寿命。</summary>
        public static bool Run(ZacurrentDragonContext ctx, int chainTime)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Roll:
                    UpdateRoll(ctx, chainTime);
                    return false;
                case Beat.Recover:
                    return UpdateRecover(ctx);
                default:
                    UpdateRetreat(ctx, chainTime);
                    return false;
            }
        }

        private static void UpdateRetreat(ZacurrentDragonContext ctx, int chainTime)
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

            // 翅膀扇完 = 出手：锁死绕飞起始角，斜切进场
            ctx.SonState = (int)Beat.Roll;
            ctx.Timer = 0;
            ctx.Recorder2 = (npc.Center - ctx.Target.Center).ToRotation() + ZacurrentDirector.ChainAnchorOffset;
            boss.ResetAllOldCaches();
            boss.canDrawShadows = true;
            boss.shadowScale = ZacurrentDirector.ChainShadowScale;
            boss.IsDashing = true;

            Vector2 dir = (ctx.Target.Center + (ctx.Recorder2.ToRotationVector2() * ZacurrentDirector.ChainDashLead) - npc.Center).SafeNormalize(Vector2.Zero);
            npc.velocity = dir * ZacurrentDirector.ChainDashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
            ctx.MarkDecision();

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched("Electric/ElectricShoot", 0.4f, 0, npc.Center);
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
                ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.ChainSkyLight, ZacurrentDirector.ChainSkyFade, ZacurrentDirector.ChainSkyExchange);
                WindCircle.Spawn(npc.Center, -dir * ZacurrentDirector.ChainWindSpeed, dir.ToRotation(), ZacurrentDragon.ZacurrentPurple,
                    ZacurrentDirector.ChainWindScale, ZacurrentDirector.ChainWindScaleMul, ZacurrentDirector.ChainWindStretch);
            }

            int first = npc.NewProjectileInAI_Server<ElectricChain>(npc.Center, Vector2.Zero, ZacurrentDirector.ChainDamage(), 0, npc.target, -1, chainTime);
            if (!VaultUtils.isClient)
            {
                ctx.Recorder = first;
            }
        }

        private static void UpdateRoll(ZacurrentDragonContext ctx, int chainTime)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            float factor = ctx.Timer / ZacurrentDirector.ChainRollFrames;
            float currentRot = ctx.Recorder2 + (factor * MathHelper.TwoPi * ZacurrentDirector.ChainRollTurns);
            Vector2 center = ctx.Target.Center + (currentRot.ToRotationVector2() * ZacurrentDirector.ChainOrbitRadius);
            Vector2 dir = center - npc.Center;

            float speed = npc.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / ZacurrentDirector.ChainApproachRange, 0, 1) * ZacurrentDirector.ChainMaxSpeed;
            npc.velocity = npc.velocity.ToRotation().AngleTowards(dir.ToRotation(), ZacurrentDirector.ChainTurnRate).ToRotationVector2()
                * Helper.Lerp(speed, aimSpeed, ZacurrentDirector.ChainSpeedLerp);
            npc.rotation = npc.velocity.ToRotation();
            ctx.DeclareDirect();

            ctx.Timer++;
            if (ctx.Timer % ZacurrentDirector.ChainSpawnInterval == 0)
            {
                // 后留的链活得更久，绕完一圈时整张网同时在场
                float life = chainTime + (ctx.Timer / ZacurrentDirector.ChainSpawnInterval * ZacurrentDirector.ChainLifeGain);
                int next = npc.NewProjectileInAI_Server<ElectricChain>(npc.Center, Vector2.Zero, ZacurrentDirector.ChainDamage(), 0, npc.target, ctx.Recorder, life);
                if (!VaultUtils.isClient)
                {
                    ctx.Recorder = next;
                }
            }

            if (ctx.Timer <= ZacurrentDirector.ChainRollFrames)
            {
                return;
            }

            ctx.SonState = (int)Beat.Recover;
            ctx.Timer = 0;
            boss.IsDashing = false;
            boss.canDrawShadows = false;
            npc.velocity *= ZacurrentDirector.ChainRollExitDamp;
            ctx.MarkDecision();
        }

        private static bool UpdateRecover(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.FlyingFrame();
            boss.SetSpriteDirectionFoTarget();
            boss.TurnToNoRot();

            float distance = npc.Center.Distance(ctx.Target.Center);
            if (distance < ZacurrentDirector.ChainNearDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.ChainRecoverNearSpeedCap)
                {
                    npc.velocity += (npc.Center - ctx.Target.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ChainRetreatAccel;
                }
            }
            else if (distance > ZacurrentDirector.ChainFarDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.ChainRecoverFarSpeedCap)
                {
                    npc.velocity += (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ChainRetreatAccel;
                }
            }
            else
            {
                npc.velocity *= ZacurrentDirector.ChainRecoverDamp;
            }

            ctx.DeclareDirect();

            ctx.Timer++;
            return ctx.Timer > ZacurrentDirector.ChainRecoverFrames;
        }
    }
}
