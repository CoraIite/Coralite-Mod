using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 追踪雷球（普通形态版，只在连段里出现，所以单发伤害比紫伏的指针电球更高）。<br/>
    /// 节拍与 <see cref="ZacurrentPointerBallMove"/> 同构：扇翅膀蓄力（紫电环，收得更慢）→ 3 波环形追踪球。<br/>
    /// 两者刻意各写一份而不是共用一个带参版本：它们的蓄力时长、翅膀速度、粒子与弹幕都不同，
    /// 合并只会让任何一次调参波及另一招（D10）。旧 <c>ZacurrentDragon.AimThunderBall</c>（AI.AimThunderBall.cs:13-85）。
    /// </summary>
    internal static class ZacurrentAimThunderBallMove
    {
        private enum Beat
        {
            /// <summary>扇翅膀蓄力（旧 SonState 0）</summary>
            Charge = 0,
            /// <summary>环形撒球（旧 SonState 1）</summary>
            Shoot = 1,
        }

        /// <summary>返回 true 表示整招结束。<paramref name="aimTime"/> 为电球的基础追踪时长。</summary>
        public static bool Run(ZacurrentDragonContext ctx, int aimTime)
        {
            if ((Beat)(int)ctx.SonState == Beat.Shoot)
            {
                return UpdateShoot(ctx, aimTime);
            }

            UpdateCharge(ctx);
            return false;
        }

        private static void UpdateCharge(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (ctx.Timer == 0)
            {
                npc.frame.Y = 1;
            }

            npc.velocity *= ZacurrentDirector.ChargeDamp;
            ctx.DeclareDirect();
            boss.SetSpriteDirectionFoTarget();
            boss.SetRotationNormally(ZacurrentDirector.ChargeRotRate);

            if (!VaultUtils.isServer)
            {
                float length = Helper.Lerp(ZacurrentDirector.ChargeRingRadiusMin, ZacurrentDirector.ChargeRingRadiusMax,
                    ctx.Timer / ZacurrentDirector.AimChargeRamp);
                for (int i = 0; i < ZacurrentDirector.ElectricParticlePerFrame; i++)
                {
                    PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2CircularEdge(length, length),
                        Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(),
                        Scale: Main.rand.NextFloat(ZacurrentDirector.ElectricParticleScaleMin, ZacurrentDirector.ElectricParticleScaleMax));
                }
            }

            ctx.Timer++;

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
            ctx.SonState = (int)Beat.Shoot;
            ctx.Timer = 0;
            boss.OpenMouse = true;
            boss.ResetAllOldCaches();
            boss.canDrawShadows = true;
            npc.velocity = Vector2.Zero;
            ctx.MarkDecision();

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, npc.Center, pitch: ZacurrentDirector.ElectricOrbPitch);
                SoundEngine.PlaySound(CoraliteSoundID.Roar, npc.Center);
            }

            ctx.Recorder = ctx.AttackRandFloat(0f, MathHelper.TwoPi);
        }

        private static bool UpdateShoot(ZacurrentDragonContext ctx, int aimTime)
        {
            NPC npc = ctx.Npc;
            ctx.Timer++;

            if (ctx.Timer % ZacurrentDirector.AimShootInterval == 0)
            {
                int damage = ZacurrentDirector.AimThunderBallDamage();
                for (int i = 0; i < ZacurrentDirector.AimShootCount; i++)
                {
                    float angle = ctx.Recorder + (ctx.Timer * ZacurrentDirector.AimSpinPerFrame)
                        + (i * MathHelper.TwoPi / ZacurrentDirector.AimShootCount)
                        + ctx.AttackRandFloat(-ZacurrentDirector.AimSpreadJitter, ZacurrentDirector.AimSpreadJitter);
                    Vector2 dir = angle.ToRotationVector2() * ctx.AttackRandFloat(ZacurrentDirector.AimBallSpeedMin, ZacurrentDirector.AimBallSpeedMax);
                    npc.NewProjectileInAI_Server<AimThunderBall>(npc.Center, dir, damage, 0, npc.target, npc.whoAmI,
                        aimTime + (ctx.Timer / ZacurrentDirector.AimTimeGainDiv));
                }

                ctx.Boss.ElectricSound();
            }

            return ctx.Timer > ZacurrentDirector.AimShootInterval * ZacurrentDirector.AimShootWaves;
        }
    }
}
