using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Content.Bosses.Rediancie;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 赤色脉冲（远程）：悬停在玩家上方，弹药聚成斜环蓄力 100 帧，之后每 45 帧从炮口打出一发追踪脉冲（每发消耗 1 弹药），
    /// 255 帧收环、275 帧收招；弹药耗尽立即收招。旧 <c>Bloodiancie.Pulse</c>（AI.cs:206-260）。
    /// 预告：炮口持续回吸宝石尘，开火段尘更大。
    /// </summary>
    [VaultState((int)BloodiancieStateId.pulse, typeof(BloodiancieContext))]
    internal sealed class BloodianciePulseState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.pulse;

        /// <summary>开火拍：100 帧起每 45 帧（两端同算）。</summary>
        private bool FireBeat => Timer >= BloodiancieDirector.PulseChargeFrames
            && (Timer - BloodiancieDirector.PulseChargeFrames) % BloodiancieDirector.PulseCycleFrames == 0;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            ctx.DeclareHoverAboveTarget();
            ctx.DeclareRotation(BloodiancieRotationMode.Normal);

            ctx.AimGeometry(out Vector2 targetDir, out float factor, out Vector2 muzzle);
            int realTime = Timer - BloodiancieDirector.PulseChargeFrames;

            if (Timer < BloodiancieDirector.PulseChargeFrames)
            {
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersPulse(Timer, muzzle, targetDir, factor, -1f,
                        BloodiancieDirector.AimRingLerpBase + (BloodiancieDirector.AimRingLerpGain * Timer / BloodiancieDirector.PulseRampFrames));
                }

                MuzzleDust(ctx, muzzle, targetDir, BloodiancieDirector.PulseChargeDustCount, BloodiancieDirector.PulseDustScale);
                return;
            }

            if (Timer < BloodiancieDirector.PulseFireEnd)
            {
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersPulse(Timer, muzzle, targetDir, factor, realTime % BloodiancieDirector.PulseCycleFrames);
                }

                float scale = BloodiancieDirector.PulseDustScale + (BloodiancieDirector.PulseDustScaleGain
                    * (realTime % BloodiancieDirector.PulseDustScalePeriod) / BloodiancieDirector.PulseRecoilNorm);
                MuzzleDust(ctx, muzzle, targetDir, 1, scale);
            }
            else
            {
                ctx.UpdateFollowersIdle(Timer, BloodiancieDirector.PulseIdleLerp);
            }

            if (FireBeat && ctx.CanDespawnFollower() && !Main.dedServ)
            {
                Helper.PlayPitched("RedJade/RedJadeBeam", BloodiancieDirector.BeamSoundVolume, 0f, ctx.Npc.Center);
            }
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer < BloodiancieDirector.PulseChargeFrames)
            {
                return ctx.FollowersEmpty ? EndAttack(ctx) : null;
            }

            if (Timer < BloodiancieDirector.PulseFireEnd && ctx.FollowersEmpty)
            {
                return EndAttack(ctx);
            }

            if (FireBeat)
            {
                if (!ctx.CanDespawnFollower())
                {
                    return EndAttack(ctx);
                }

                ctx.AimGeometry(out _, out _, out Vector2 muzzle);
                Vector2 velocity = (ctx.Target.Center - ctx.Npc.Center
                    + Main.rand.NextVector2CircularEdge(BloodiancieDirector.PulseSpread, BloodiancieDirector.PulseSpread))
                    .SafeNormalize(Vector2.UnitY) * BloodiancieDirector.PulseSpeed;
                // owner 传的是目标玩家序号（旧代码的第 7 个实参），原样保留
                Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), muzzle, velocity,
                    ModContent.ProjectileType<RedPulse>(), BloodiancieDirector.PulseDamage(), BloodiancieDirector.PulseKnockback, ctx.Npc.target);

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > BloodiancieDirector.PulseEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>炮口回吸尘（纯本地）。旧 AI.cs:226,232</summary>
        private static void MuzzleDust(BloodiancieContext ctx, Vector2 muzzle, Vector2 targetDir, int count, float scale)
        {
            if (Main.dedServ)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                Helper.SpawnTrailDust(muzzle + Main.rand.NextVector2Circular(BloodiancieDirector.PulseDustSpread, BloodiancieDirector.PulseDustSpread),
                    DustID.GemRuby, (d) => -targetDir * BloodiancieDirector.PulseDustSpeed, Scale: scale);
            }
        }
    }
}
