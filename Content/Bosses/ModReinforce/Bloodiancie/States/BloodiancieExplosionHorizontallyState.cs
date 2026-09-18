using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 横向爆炸（大师近战）：160 帧里维持 300~350 px 的横向站位并跟住高度，170 帧在前方炸开并贴地送出一道血浪，
    /// 同时补 8 发弹药，210 帧收招。旧 <c>Bloodiancie.ExplosionHorizontally</c>（AI.cs:365-413）。
    /// 预告：站位段每 5 帧撒尘、越站越密。
    /// </summary>
    [VaultState((int)BloodiancieStateId.explosionHorizontally, typeof(BloodiancieContext))]
    internal sealed class BloodiancieExplosionHorizontallyState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.explosionHorizontally;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer < BloodiancieDirector.HorizontalChaseFrames)
            {
                ctx.DeclareChaseXBand(BloodiancieDirector.HorizontalKeepNear, BloodiancieDirector.HorizontalKeepFar,
                    BloodiancieDirector.HorizontalSpeedX, BloodiancieDirector.HorizontalAccelX,
                    BloodiancieDirector.HorizontalTurnX, BloodiancieDirector.HorizontalDamp);
                ctx.DeclareChaseYWithDeadZone(BloodiancieDirector.HorizontalSpeedY, BloodiancieDirector.HorizontalAccelY,
                    BloodiancieDirector.HorizontalTurnY, BloodiancieDirector.HorizontalDamp);

                if (Timer % BloodiancieDirector.HorizontalDustInterval == 0)
                {
                    ChargeDust(ctx, Timer / BloodiancieDirector.HorizontalDustPerFrames, BloodiancieDirector.ChargeDustScaleGain);
                }
            }
            else
            {
                ctx.DeclareDamp(BloodiancieDirector.HorizontalSettleDamp);
            }

            // 补 8 发弹药（两端同算，数量随包纠正）
            if (Timer == BloodiancieDirector.HorizontalBoomFrame)
            {
                ctx.SpawnFollowers(BloodiancieDirector.HorizontalGainFollowers);
            }

            ctx.DeclareRotation(BloodiancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer == BloodiancieDirector.HorizontalBoomFrame)
            {
                SpawnBigBoom(ctx, Ahead(ctx), BloodiancieDirector.HorizontalBoomDamage(), BloodiancieDirector.HorizontalBoomKnockback);

                Vector2 waveVelocity = new Vector2(ctx.Target.Center.X > ctx.Npc.Center.X ? 1 : -1, 0) * BloodiancieDirector.HorizontalWaveSpeed;
                Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), ctx.Npc.Center, waveVelocity,
                    ModContent.ProjectileType<BloodWave>(), 1, BloodiancieDirector.HorizontalWaveKnockback);
            }

            if (Timer > BloodiancieDirector.HorizontalEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
