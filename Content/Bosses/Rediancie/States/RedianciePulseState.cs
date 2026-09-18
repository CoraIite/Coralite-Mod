using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 赤色脉冲（大师二阶段远程）：悬停在玩家上方，弹药聚成朝向玩家的斜环蓄力 125 帧，之后每 65 帧从炮口射一发大型赤玉弹幕（每发消耗 1 弹药），
    /// 255 帧后收环，275 帧收招；弹药耗尽立即收招。旧 <c>Rediancie.Pulse</c>（Rediancie.cs:452-506）。
    /// 预告：炮口尘越蓄越大；蓄力段弹药环持续收拢即承诺。
    /// </summary>
    [VaultState((int)RediancieStateId.pulse, typeof(RediancieContext))]
    internal sealed class RedianciePulseState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.pulse;

        /// <summary>本帧是否到了出手拍（realTime % 65 == 0，蓄力段除外），两端同算。</summary>
        private bool FireBeat => Timer >= RediancieDirector.PulseChargeFrames
            && (Timer - RediancieDirector.PulseRampFrames) % RediancieDirector.PulseCycleFrames == 0;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareHoverAboveTarget();
            ctx.DeclareRotation(RediancieRotationMode.Normal);

            ctx.AimGeometry(out Vector2 targetDir, out float factor, out Vector2 muzzle);
            int realTime = Timer - RediancieDirector.PulseRampFrames;

            if (Timer < RediancieDirector.PulseChargeFrames)
            {
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersPulse(Timer, muzzle, targetDir, factor, -1,
                        RediancieDirector.FireworkLerpBase + (RediancieDirector.FireworkLerpGain * Timer / RediancieDirector.PulseRampFrames));
                }

                if (!Main.dedServ)
                {
                    for (int i = 0; i < RediancieDirector.PulseChargeDustCount; i++)
                    {
                        Helper.SpawnTrailDust(muzzle + Main.rand.NextVector2Circular(RediancieDirector.PulseDustSpread, RediancieDirector.PulseDustSpread),
                            DustID.GemRuby, (d) => -targetDir * RediancieDirector.PulseDustSpeed, Scale: RediancieDirector.PulseDustScale);
                    }
                }

                return;
            }

            if (Timer < RediancieDirector.PulseFireEnd)
            {
                float cycle = realTime % RediancieDirector.PulseCycleFrames;
                if (!ctx.FollowersEmpty)
                {
                    ctx.UpdateFollowersPulse(Timer, muzzle, targetDir, factor, cycle);
                }

                if (!Main.dedServ)
                {
                    Helper.SpawnTrailDust(muzzle + Main.rand.NextVector2Circular(RediancieDirector.PulseDustSpread, RediancieDirector.PulseDustSpread),
                        DustID.GemRuby, (d) => -targetDir * RediancieDirector.PulseDustSpeed,
                        Scale: RediancieDirector.PulseDustScale + (RediancieDirector.PulseDustScaleGain * cycle / RediancieDirector.PulseCycleFrames));
                }
            }
            else
            {
                ctx.UpdateFollowersIdle(Timer, RediancieDirector.PulseIdleLerp);
            }

            // 出手音效两端各放（旧代码在服务端守卫外，客户端也响）
            if (FireBeat && ctx.CanDespawnFollower() && !Main.dedServ)
            {
                Helper.PlayPitched("RedJade/RedJadeBeam", RediancieDirector.BeamSoundVolume, 0f, ctx.Npc.Center);
            }
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            // 旧代码：蓄力段弹药已空 → UpdateFollower_Pulse 内部收招
            if (Timer < RediancieDirector.PulseChargeFrames)
            {
                return ctx.FollowersEmpty ? EndAttack(ctx) : null;
            }

            if (Timer < RediancieDirector.PulseFireEnd && ctx.FollowersEmpty)
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
                Vector2 velocity = (ctx.Target.Center - ctx.Npc.Center + Main.rand.NextVector2CircularEdge(RediancieDirector.PulseSpread, RediancieDirector.PulseSpread))
                    .SafeNormalize(Vector2.UnitY) * RediancieDirector.PulseSpeed;
                Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), muzzle, velocity,
                    ModContent.ProjectileType<RedPulse>(), RediancieDirector.PulseDamage(), RediancieDirector.PulseKnockback, ctx.Npc.target);

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > RediancieDirector.PulseEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
