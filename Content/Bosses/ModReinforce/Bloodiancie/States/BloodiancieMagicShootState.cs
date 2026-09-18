using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 赤玉激光（远程）：悬停在玩家上方，弹药聚成斜环蓄力 60 帧，之后每 15 帧从炮口射一道血玉激光（每发消耗 1 弹药），
    /// 140 帧后收环，155 帧收招；弹药耗尽立即收招。旧 <c>Bloodiancie.MagicShoot</c>（AI.cs:686-738）。
    /// 预告：每 3 帧 6 粒宝石尘从炮口回吸向本体，蓄力越久环越收紧。
    /// </summary>
    [VaultState((int)BloodiancieStateId.magicShoot, typeof(BloodiancieContext))]
    internal sealed class BloodiancieMagicShootState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.magicShoot;

        /// <summary>出手拍：60 帧起每 15 帧（两端同算）。</summary>
        private bool FireBeat => Timer >= BloodiancieDirector.MagicChargeFrames && Timer % BloodiancieDirector.MagicInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            ctx.DeclareHoverAboveTarget();
            ctx.DeclareRotation(BloodiancieRotationMode.Normal);

            ctx.AimGeometry(out Vector2 targetDir, out float factor, out Vector2 muzzle);

            if (Timer % BloodiancieDirector.MagicDustInterval == 0 && !Main.dedServ)
            {
                for (int i = 0; i < BloodiancieDirector.MagicDustCount; i++)
                {
                    Helper.SpawnTrailDust(muzzle + Main.rand.NextVector2Circular(BloodiancieDirector.MagicDustSpread, BloodiancieDirector.MagicDustSpread),
                        DustID.GemRuby, (dust) => (ctx.Npc.Center - dust.position).SafeNormalize(Vector2.UnitY) * BloodiancieDirector.MagicDustSpeed,
                        Scale: BloodiancieDirector.MagicDustScale);
                }
            }

            if (Timer < BloodiancieDirector.MagicChargeFrames)
            {
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersMagicShoot(Timer, muzzle, targetDir, factor,
                        BloodiancieDirector.AimRingLerpBase + (BloodiancieDirector.AimRingLerpGain * Timer / BloodiancieDirector.MagicChargeFrames));
                }

                return;
            }

            if (Timer < BloodiancieDirector.MagicAimEnd)
            {
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersMagicShoot(Timer, muzzle, targetDir, factor);
                }
            }
            else
            {
                ctx.UpdateFollowersIdle(Timer, BloodiancieDirector.MagicIdleLerp);
            }

            if (FireBeat && ctx.CanDespawnFollower() && !Main.dedServ)
            {
                Helper.PlayPitched("RedJade/RedJadeBeam", BloodiancieDirector.BeamSoundVolume, 0f, ctx.Npc.Center);
            }
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer < BloodiancieDirector.MagicChargeFrames)
            {
                return ctx.FollowersEmpty ? EndAttack(ctx) : null;
            }

            if (Timer < BloodiancieDirector.MagicAimEnd && ctx.FollowersEmpty)
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
                // 旧表达式 new Vector2(0, 30 * (Timer / 30) == 1 ? 1 : -1) 的运算优先级使 Y 恒为 -1，原样保留（写进报告“建议”）
                Vector2 velocity = (ctx.Target.Center - ctx.Npc.Center + new Vector2(0, -1)).SafeNormalize(Vector2.UnitY) * BloodiancieDirector.MagicSpeed;
                Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), muzzle, velocity,
                    ModContent.ProjectileType<BloodiancieBeam>(), BloodiancieDirector.MagicDamage(), BloodiancieDirector.MagicKnockback);

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > BloodiancieDirector.MagicEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
