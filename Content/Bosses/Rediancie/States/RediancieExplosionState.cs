using Coralite.Content.Bosses.Rediancie.Core;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 三连炸（近战基础招）：追逐玩家，每 80 帧在前方放一个爆炸并获得 1 发弹药，250 帧收招；
    /// 大师模式在 125 / 187 帧比玩家高时 1/2 概率改下砸。旧 <c>Rediancie.Explosion</c>（Rediancie.cs:705-760）。
    /// 预告：每 3 帧撒尘、随 80 帧周期由疏到密。
    /// </summary>
    [VaultState((int)RediancieStateId.explosion, typeof(RediancieContext))]
    internal sealed class RediancieExplosionState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.explosion;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareChaseX(RediancieDirector.ExplosionSpeedX, RediancieDirector.ExplosionAccelX, RediancieDirector.ExplosionTurnX, RediancieDirector.ExplosionDamp);
            ctx.DeclareChaseYWithDeadZone(RediancieDirector.ExplosionSpeedY, RediancieDirector.ExplosionAccelY, RediancieDirector.ExplosionTurnY, RediancieDirector.ExplosionDamp);
            ctx.DeclareRotation(RediancieRotationMode.Normal);

            if (Timer % RediancieDirector.ExplosionDustInterval == 0)
            {
                ChargeDust(ctx, Timer % RediancieDirector.ExplosionInterval / RediancieDirector.ExplosionDustDivisor, RediancieDirector.ExplosionDustScaleGain);
            }

            // 每 80 帧获得 1 发弹药（两端同算，数量随包纠正）
            if (Timer % RediancieDirector.ExplosionInterval == 0)
            {
                ctx.SpawnFollowers(RediancieDirector.ExplosionGainFollowers);
            }

            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (Main.masterMode && Array.IndexOf(RediancieDirector.ExplosionSlamCheckFrames, Timer) >= 0
                && AboveTarget(ctx) && Main.rand.NextBool(RediancieDirector.ExplosionSlamChance))
            {
                return Create(RediancieStateId.slamDown);
            }

            if (Timer % RediancieDirector.ExplosionInterval == 0)
            {
                Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), Ahead(ctx), Vector2.Zero,
                    ModContent.ProjectileType<Rediancie_Explosion>(), RediancieDirector.ExplosionDamage(), RediancieDirector.ExplosionKnockback);
            }

            if (Timer > RediancieDirector.ExplosionEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
