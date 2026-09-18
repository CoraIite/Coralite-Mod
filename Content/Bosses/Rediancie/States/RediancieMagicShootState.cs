using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 赤玉激光（远程）：悬停在玩家上方，弹药聚成斜环蓄力 60 帧，每 35 帧从炮口射一道激光（每发消耗 1 弹药），
    /// 140 帧后收环，155 帧收招；弹药耗尽立即收招。旧 <c>Rediancie.MagicShoot</c>（Rediancie.cs:803-855）。
    /// 预告：每 3 帧 6 粒回吸尘汇向本体。
    /// </summary>
    [VaultState((int)RediancieStateId.magicShoot, typeof(RediancieContext))]
    internal sealed class RediancieMagicShootState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.magicShoot;

        /// <summary>出手拍：60 帧起每 35 帧（两端同算）。</summary>
        private bool FireBeat => Timer >= RediancieDirector.MagicChargeFrames && Timer % RediancieDirector.MagicInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareHoverAboveTarget();
            ctx.DeclareRotation(RediancieRotationMode.Normal);

            ctx.AimGeometry(out Vector2 targetDir, out float factor, out Vector2 muzzle);

            if (Timer % RediancieDirector.MagicDustInterval == 0 && !Main.dedServ)
            {
                for (int i = 0; i < RediancieDirector.MagicDustCount; i++)
                {
                    Helper.SpawnTrailDust(muzzle + Main.rand.NextVector2Circular(RediancieDirector.MagicDustSpread, RediancieDirector.MagicDustSpread), DustID.GemRuby,
                        (dust) => (ctx.Npc.Center - dust.position).SafeNormalize(Vector2.UnitY) * RediancieDirector.MagicDustSpeed, Scale: RediancieDirector.MagicDustScale);
                }
            }

            if (Timer < RediancieDirector.MagicChargeFrames)
            {
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersMagicShoot(Timer, muzzle, targetDir, factor,
                        RediancieDirector.FireworkLerpBase + (RediancieDirector.FireworkLerpGain * Timer / RediancieDirector.MagicChargeFrames));
                }

                return;
            }

            if (Timer < RediancieDirector.MagicAimEnd)
            {
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersMagicShoot(Timer, muzzle, targetDir, factor);
                }
            }
            else
            {
                ctx.UpdateFollowersIdle(Timer, RediancieDirector.MagicIdleLerp);
            }

            if (FireBeat && ctx.CanDespawnFollower() && !Main.dedServ)
            {
                Helper.PlayPitched("RedJade/RedJadeBeam", RediancieDirector.BeamSoundVolume, 0f, ctx.Npc.Center);
            }
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (Timer < RediancieDirector.MagicChargeFrames)
            {
                return ctx.FollowersEmpty ? EndAttack(ctx) : null;
            }

            if (Timer < RediancieDirector.MagicAimEnd && ctx.FollowersEmpty)
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
                // 旧表达式 new Vector2(0, 60 * (Timer / 30) == 1 ? 1 : -1) 的运算优先级使 Y 恒为 -1，原样保留（写进报告“建议”）
                Vector2 velocity = (ctx.Target.Center - ctx.Npc.Center + new Vector2(0, -1)).SafeNormalize(Vector2.UnitY) * RediancieDirector.MagicSpeed;
                Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), muzzle, velocity,
                    ModContent.ProjectileType<Rediancie_Beam>(), RediancieDirector.MagicDamage(), RediancieDirector.MagicKnockback);

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > RediancieDirector.MagicEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
