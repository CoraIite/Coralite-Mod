using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 聚集电流（每条连段的收尾件）：<br/>
    /// Charge —— 扇一轮翅膀，扇完开始蓄电并进入无敌。<br/>
    /// Spawn —— 240 帧里绕着自己撒下一圈圈紫电球（每 10 帧一个，角度螺旋推进）与红电球（每 40 帧一个）；
    /// 玩家打碎它们就给龙充紫电，充满就进紫伏形态——这是本 boss 的核心资源循环。
    /// 无敌在撒球结束前 60 帧解除，那 60 帧就是明确的"可以打了"信号，之后还有 5 秒电球滞留期。<br/>
    /// Recover —— 30 帧后摇收招。<br/>
    /// 旧 <c>ZacurrentDragon.GatherCurrent</c>（AI.GatherCurrent.cs:11-140）。
    /// </summary>
    internal static class ZacurrentGatherCurrentMove
    {
        private enum Beat
        {
            /// <summary>扇翅膀蓄力（旧 SonState 0）</summary>
            Charge = 0,
            /// <summary>撒电球（旧 SonState 1）</summary>
            Spawn = 1,
            /// <summary>后摇（旧 SonState 2）</summary>
            Recover = 2,
        }

        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Spawn:
                    UpdateSpawn(ctx);
                    return false;
                case Beat.Recover:
                    ctx.Boss.FlyingFrame();
                    ctx.Boss.SetSpriteDirectionFoTarget();
                    ctx.Boss.TurnToNoRot();
                    ctx.Timer++;
                    return ctx.Timer > ZacurrentDirector.GatherRecoverFrames;
                default:
                    UpdateCharge(ctx);
                    return false;
            }
        }

        private static void UpdateCharge(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (ctx.Timer == 0)
            {
                npc.frame.Y = 1;
                ctx.Timer = 1;
            }

            npc.velocity *= ZacurrentDirector.ChargeDamp;
            ctx.DeclareDirect();
            boss.SetSpriteDirectionFoTarget();
            boss.SetRotationNormally(ZacurrentDirector.ChargeRotRate);

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

            npc.frame.Y = 0;
            ctx.SonState = (int)Beat.Spawn;
            ctx.Timer = 0;
            boss.OpenMouse = true;
            boss.ResetAllOldCaches();
            boss.canDrawShadows = true;
            boss.currentSurrounding = true;
            npc.velocity = Vector2.Zero;
            ctx.Invulnerable = true;
            ctx.MarkDecision();

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, npc.Center, pitch: ZacurrentDirector.ElectricOrbPitch);
                SoundEngine.PlaySound(CoraliteSoundID.Roar, npc.Center);
            }

            // 两条撒球螺旋的起始角（两端从同一 AttackSeed 派生）
            ctx.Recorder = ctx.AttackRandFloat(0f, MathHelper.TwoPi);
            ctx.Recorder2 = ctx.AttackRandFloat(0f, MathHelper.TwoPi);
        }

        private static void UpdateSpawn(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            float factor = ctx.Timer / ZacurrentDirector.GatherMaxFrames;
            boss.shadowScale = Helper.Lerp(1f, ZacurrentDirector.GatherShadowScaleMax, factor * ZacurrentDirector.GatherShadowCycles % 1);
            boss.shadowAlpha = Helper.Lerp(1f, 0f, factor * ZacurrentDirector.GatherShadowCycles % 1);

            ctx.Timer++;

            // 撒球结束前 60 帧解除无敌：这一段是明确给玩家的反击窗口
            ctx.Invulnerable = ctx.Timer < ZacurrentDirector.GatherSpawnFrames - ZacurrentDirector.GatherInvulEndLead;

            SpawnDust(ctx);

            if (ctx.Timer < ZacurrentDirector.GatherSpawnFrames)
            {
                if (ctx.Timer % ZacurrentDirector.GatherVoltBallInterval == 0)
                {
                    ctx.Recorder += ZacurrentDirector.GatherVoltBallAngleStep;
                    int length = ctx.AttackRandom.Next(ZacurrentDirector.GatherVoltBallRadiusMin, ZacurrentDirector.GatherVoltBallRadiusMax);
                    Vector2 pos = npc.Center + (ctx.Recorder.ToRotationVector2() * length);

                    if (!VaultUtils.isClient)
                    {
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)pos.X, (int)pos.Y, ModContent.NPCType<PurpleVoltBall>(), ai0: npc.whoAmI);
                    }
                }

                if (ctx.Timer % (ZacurrentDirector.GatherVoltBallInterval * ZacurrentDirector.GatherRedBallIntervalMul) == 0)
                {
                    ctx.Recorder2 += ZacurrentDirector.GatherRedBallAngleStep;
                    int length = ctx.AttackRandom.Next(ZacurrentDirector.GatherRedBallRadiusMin, ZacurrentDirector.GatherRedBallRadiusMax);
                    Vector2 pos = npc.Center + (ctx.Recorder2.ToRotationVector2() * length);

                    npc.NewProjectileInAI_Server<RedVoltBall>(pos, Vector2.Zero, ZacurrentDirector.GatherRedBallDamage(), 0, npc.target, npc.whoAmI);

                    if (!VaultUtils.isServer)
                    {
                        Helper.PlayPitched(CoraliteSoundID.QuietElectric_DD2_LightningAuraZap, npc.Center);
                    }
                }
            }

            if (ctx.Timer <= ZacurrentDirector.GatherMaxFrames)
            {
                return;
            }

            ctx.SonState = (int)Beat.Recover;
            ctx.Timer = 0;
            boss.canDrawShadows = false;
            boss.OpenMouse = false;
            ctx.MarkDecision();
        }

        /// <summary>纯表现：向内收束的电尘，半径随进度一路缩小，视觉上"把整片场地的电吸过来"。</summary>
        private static void SpawnDust(ZacurrentDragonContext ctx)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            NPC npc = ctx.Npc;
            float progress = ctx.Timer / ZacurrentDirector.GatherMaxFrames;

            if (ctx.Timer % ZacurrentDirector.GatherDustInterval == 0)
            {
                float radius = ZacurrentDirector.GatherDustRadius - (progress * ZacurrentDirector.GatherDustRadiusShrink);
                Vector2 dir = Helper.NextVec2Dir();
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * (radius + Main.rand.Next(1, ZacurrentDirector.GatherDustJitter))),
                    DustID.PortalBoltTrail, -dir * Main.rand.NextFloat(ZacurrentDirector.GatherDustSpeedMin, ZacurrentDirector.GatherDustSpeedMax),
                    newColor: ZacurrentDragon.ZacurrentDustRed,
                    Scale: Main.rand.NextFloat(ZacurrentDirector.GatherDustScaleMin, ZacurrentDirector.GatherDustScaleMax));
                dust.noGravity = true;
            }

            if (ctx.Timer % ZacurrentDirector.GatherRingInterval != 0)
            {
                return;
            }

            float ringRadius = ZacurrentDirector.GatherDustRadius - (progress * ZacurrentDirector.GatherRingRadiusShrink);
            for (int i = 0; i < ZacurrentDirector.GatherRingDustCount; i++)
            {
                Vector2 dir = (i * MathHelper.TwoPi / ZacurrentDirector.GatherRingDustCount).ToRotationVector2();
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * (ringRadius + Main.rand.Next(1, ZacurrentDirector.GatherDustJitter))),
                    DustID.PortalBoltTrail, -dir * Main.rand.NextFloat(ZacurrentDirector.GatherRingDustSpeedMin, ZacurrentDirector.GatherRingDustSpeedMax),
                    newColor: ZacurrentDragon.ZacurrentDustPurple,
                    Scale: Main.rand.NextFloat(ZacurrentDirector.GatherRingDustScaleMin, ZacurrentDirector.GatherRingDustScaleMax));
                dust.noGravity = true;
            }
        }
    }
}
