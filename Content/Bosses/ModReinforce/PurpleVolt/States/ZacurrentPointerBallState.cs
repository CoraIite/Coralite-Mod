using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 指针电球（紫伏专属）：<br/>
    /// Charge —— 原地扇一整轮翅膀蓄力，红电环从 750 px 收到 80 px，环收完即"要出手了"的预告；翅膀走完一圈转入下一拍。<br/>
    /// Shoot —— 每 15 帧一波、每波 3 颗环形铺开的追踪电球，共 3 波；起始角由 <see cref="ZacurrentDragonContext.Recorder"/> 锁死，
    /// 每帧再缓慢自旋，保证三波不重叠。后发的球追踪时间更长，同批大致同时抵达。<br/>
    /// 旧 <c>ZacurrentDragon.PointerBallP2</c>（AI.PointerBall.cs:11-83）。
    /// </summary>
    internal static class ZacurrentPointerBallMove
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
                    ctx.Timer / ZacurrentDirector.PointerChargeRamp);
                for (int i = 0; i < ZacurrentDirector.ElectricParticlePerFrame; i++)
                {
                    PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2CircularEdge(length, length),
                        Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(),
                        Scale: Main.rand.NextFloat(ZacurrentDirector.ElectricParticleScaleMin, ZacurrentDirector.ElectricParticleScaleMax));
                }
            }

            ctx.Timer++;

            // 向后扇一下翅膀，帧图走完一轮就转入撒球
            if (++npc.frameCounter <= ZacurrentDirector.PointerWingFrameTime)
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

            // 锁死本次的环形起始角（两端从同一 AttackSeed 派生）
            ctx.Recorder = ctx.AttackRandFloat(0f, MathHelper.TwoPi);
        }

        private static bool UpdateShoot(ZacurrentDragonContext ctx, int aimTime)
        {
            NPC npc = ctx.Npc;
            ctx.Timer++;

            if (ctx.Timer % ZacurrentDirector.AimShootInterval == 0)
            {
                int damage = ZacurrentDirector.PointerBallDamage();
                for (int i = 0; i < ZacurrentDirector.AimShootCount; i++)
                {
                    float angle = ctx.Recorder + (ctx.Timer * ZacurrentDirector.AimSpinPerFrame)
                        + (i * MathHelper.TwoPi / ZacurrentDirector.AimShootCount)
                        + ctx.AttackRandFloat(-ZacurrentDirector.AimSpreadJitter, ZacurrentDirector.AimSpreadJitter);
                    Vector2 dir = angle.ToRotationVector2() * ctx.AttackRandFloat(ZacurrentDirector.AimBallSpeedMin, ZacurrentDirector.AimBallSpeedMax);
                    npc.NewProjectileInAI_Server<AimThunderBallRed>(npc.Center, dir, damage, 0, npc.target, npc.whoAmI,
                        aimTime + (ctx.Timer / ZacurrentDirector.AimTimeGainDiv));
                }

                ctx.Boss.ElectricSound();
            }

            return ctx.Timer > ZacurrentDirector.AimShootInterval * ZacurrentDirector.AimShootWaves;
        }
    }

    /// <summary>单招版指针电球：追踪时长取 <see cref="ZacurrentDirector.PointerBallAimTime"/>。旧壳 ZacurrentDragon.States.cs:230-234</summary>
    [VaultState((int)ZacurrentDragon.AIStates.PointerBall, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentPointerBallState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.PointerBall;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
            => ZacurrentPointerBallMove.Run(ctx, ZacurrentDirector.PointerBallAimTime);
    }
}
