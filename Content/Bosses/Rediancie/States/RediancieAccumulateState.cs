using Coralite.Content.Bosses.Rediancie.Core;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 蓄力大爆炸（大师近战）：以较快速度追逐玩家 325 帧、尘越蓄越密，之后刹停；330 帧在前方大爆炸 + 震屏并获得 6 发弹药，340 帧收招。
    /// 85 / 125 / 187 帧比玩家高时 1/2 概率改下砸。旧 <c>Rediancie.Accumulate</c>（Rediancie.cs:564-631）。
    /// </summary>
    [VaultState((int)RediancieStateId.accumulate, typeof(RediancieContext))]
    internal sealed class RediancieAccumulateState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.accumulate;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (Timer < RediancieDirector.AccumulateChaseFrames)
            {
                ctx.DeclareChaseX(RediancieDirector.AccumulateSpeedX, RediancieDirector.AccumulateAccelX, RediancieDirector.AccumulateTurnX, RediancieDirector.AccumulateDamp);
                ctx.DeclareChaseYWithDeadZone(RediancieDirector.AccumulateSpeedY, RediancieDirector.AccumulateAccelY, RediancieDirector.AccumulateTurnY, RediancieDirector.AccumulateDamp);

                if (Timer % RediancieDirector.AccumulateDustInterval == 0)
                {
                    ChargeDust(ctx, Timer / RediancieDirector.SpawnAnimDustPerFrames, RediancieDirector.SpawnAnimDustScaleGain);
                }
            }
            else
            {
                ctx.DeclareDamp(RediancieDirector.AccumulateSettleDamp);
            }

            if (Timer == RediancieDirector.AccumulateBoomFrame)
            {
                Shake(ctx, RediancieDirector.BoomShakeStrength, RediancieDirector.BoomShakeVibration, RediancieDirector.BoomShakeFrames);
                ctx.SpawnFollowers(RediancieDirector.AccumulateGainFollowers);
            }

            ctx.DeclareRotation(RediancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (Array.IndexOf(RediancieDirector.AccumulateSlamCheckFrames, Timer) >= 0
                && AboveTarget(ctx) && Main.rand.NextBool(RediancieDirector.AccumulateSlamChance))
            {
                return Create(RediancieStateId.slamDown);
            }

            if (Timer == RediancieDirector.AccumulateBoomFrame)
            {
                SpawnBigBoom(ctx, Ahead(ctx), RediancieDirector.AccumulateBoomDamage(), RediancieDirector.AccumulateBoomKnockback);
            }

            if (Timer > RediancieDirector.AccumulateEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
