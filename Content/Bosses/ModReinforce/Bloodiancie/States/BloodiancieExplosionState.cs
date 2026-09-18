using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Content.Bosses.Rediancie;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 多段爆炸（近战基础招，也是出场后的固定首招）：<br/>
    /// 前段 210 帧追着玩家、每 70 帧在前方一记大爆炸并补 2 发弹药；后段加速追击到 310 帧、每 10 帧一个小爆炸；
    /// 310 帧收尾大爆炸 + 补 8 发弹药，随后 0.9 硬刹到 320 帧收招。旧 <c>Bloodiancie.Explosion</c>（AI.cs:483-569）。
    /// 预告：前段的尘随 70 帧周期由疏到密，后段越冲越密。
    /// </summary>
    [VaultState((int)BloodiancieStateId.explosion, typeof(BloodiancieContext))]
    internal sealed class BloodiancieExplosionState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.explosion;

        /// <summary>前段：追击 + 每 70 帧大爆炸。</summary>
        private bool FirstStage => Timer <= BloodiancieDirector.ExplosionFirstStage;

        /// <summary>后段：加速追击 + 每 10 帧小爆炸。</summary>
        private bool RushStage => Timer > BloodiancieDirector.ExplosionFirstStage && Timer < BloodiancieDirector.ExplosionSecondStage;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (FirstStage)
            {
                ctx.DeclareChaseX(BloodiancieDirector.ExplosionSpeedX, BloodiancieDirector.ExplosionAccelX,
                    BloodiancieDirector.ExplosionTurnX, BloodiancieDirector.ExplosionDamp);
                ctx.DeclareChaseYWithDeadZone(BloodiancieDirector.ExplosionSpeedY, BloodiancieDirector.ExplosionAccelY,
                    BloodiancieDirector.ExplosionTurnY, BloodiancieDirector.ExplosionDamp);

                if (Timer % BloodiancieDirector.ExplosionDustInterval == 0)
                {
                    ChargeDust(ctx, Timer % BloodiancieDirector.ExplosionInterval / BloodiancieDirector.ExplosionDustDivisor,
                        BloodiancieDirector.ExplosionDustScaleGain);
                }

                // 每 70 帧补 2 发弹药（两端同算，数量随包纠正）
                if (Timer % BloodiancieDirector.ExplosionInterval == 0)
                {
                    ctx.SpawnFollowers(BloodiancieDirector.ExplosionGainFollowers);
                }
            }
            else if (RushStage)
            {
                ctx.DeclareChaseX(BloodiancieDirector.ExplosionRushSpeedX, BloodiancieDirector.ExplosionRushAccelX,
                    BloodiancieDirector.ExplosionRushTurnX, BloodiancieDirector.ExplosionDamp);
                ctx.DeclareChaseYWithDeadZone(BloodiancieDirector.ExplosionRushSpeedY, BloodiancieDirector.ExplosionRushAccelY,
                    BloodiancieDirector.ExplosionRushTurnY, BloodiancieDirector.ExplosionDamp);

                if (Timer % BloodiancieDirector.ExplosionRushDustInterval == 0)
                {
                    ChargeDust(ctx, Timer / BloodiancieDirector.ExplosionRushDustPerFrames,
                        BloodiancieDirector.ExplosionRushDustScaleGain, BloodiancieDirector.ChargeDustSpreadNarrow);
                }
            }
            else
            {
                // 收尾：310 帧补 8 发弹药后一路硬刹
                if (Timer == BloodiancieDirector.ExplosionSecondStage)
                {
                    ctx.SpawnFollowers(BloodiancieDirector.ExplosionFinalGainFollowers);
                }

                ctx.DeclareDamp(BloodiancieDirector.ExplosionBrakeDamp);
            }

            ctx.DeclareRotation(BloodiancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (FirstStage)
            {
                if (Timer % BloodiancieDirector.ExplosionInterval == 0)
                {
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), Ahead(ctx), Vector2.Zero,
                        ModContent.ProjectileType<Rediancie_BigBoom>(), BloodiancieDirector.ExplosionBoomDamage(), BloodiancieDirector.ExplosionBoomKnockback);
                }
            }
            else if (RushStage)
            {
                if (Timer % BloodiancieDirector.ExplosionRushInterval == 0)
                {
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), Ahead(ctx), Vector2.Zero,
                        ModContent.ProjectileType<Rediancie_Explosion>(), BloodiancieDirector.ExplosionRushDamage(), BloodiancieDirector.ExplosionRushKnockback);
                }
            }
            else if (Timer == BloodiancieDirector.ExplosionSecondStage)
            {
                SpawnBigBoom(ctx, Ahead(ctx), BloodiancieDirector.ExplosionFinalDamage(), BloodiancieDirector.ExplosionFinalKnockback);
            }

            if (Timer >= BloodiancieDirector.ExplosionEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
