using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 血雨（远程，仅二阶段大师）：追着玩家蓄力 150 帧，160 帧在前方炸开并横向抛出血球（血球自己爆成血雨），同时补 8 发弹药，180 帧收招。
    /// 旧 <c>Bloodiancie.BloodRain</c>（AI.cs:262-307）。预告：蓄力段每 5 帧撒尘、越来越密。
    /// </summary>
    [VaultState((int)BloodiancieStateId.bloodRain, typeof(BloodiancieContext))]
    internal sealed class BloodiancieBloodRainState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.bloodRain;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer < BloodiancieDirector.BloodRainChaseFrames)
            {
                ctx.DeclareChaseXBeyond(BloodiancieDirector.ChaseDeadZone, BloodiancieDirector.BloodRainSpeedX,
                    BloodiancieDirector.BloodRainAccelX, BloodiancieDirector.BloodRainTurnX, BloodiancieDirector.BloodRainDamp);
                ctx.DeclareChaseYWithDeadZone(BloodiancieDirector.BloodRainSpeedY, BloodiancieDirector.BloodRainAccelY,
                    BloodiancieDirector.BloodRainTurnY, BloodiancieDirector.BloodRainDamp);

                if (Timer % BloodiancieDirector.BloodRainDustInterval == 0)
                {
                    ChargeDust(ctx, Timer / BloodiancieDirector.BloodRainDustPerFrames, BloodiancieDirector.ChargeDustScaleGain);
                }
            }
            else
            {
                ctx.DeclareDamp(BloodiancieDirector.BloodRainSettleDamp);
            }

            // 补 8 发弹药（两端同算，数量随包纠正）
            if (Timer == BloodiancieDirector.BloodRainBoomFrame)
            {
                ctx.SpawnFollowers(BloodiancieDirector.BloodRainGainFollowers);
            }

            ctx.DeclareRotation(BloodiancieRotationMode.Normal);
            ctx.UpdateFollowersFirework(Timer);
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer == BloodiancieDirector.BloodRainBoomFrame)
            {
                SpawnBigBoom(ctx, Ahead(ctx), BloodiancieDirector.BloodRainBoomDamage(), BloodiancieDirector.BloodRainBoomKnockback);

                Vector2 ballVelocity = new Vector2(ctx.Target.Center.X > ctx.Npc.Center.X ? 1 : -1, 0) * BloodiancieDirector.BloodRainBallSpeed;
                Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), ctx.Npc.Center, ballVelocity,
                    ModContent.ProjectileType<BloodBall>(), 1, BloodiancieDirector.BloodRainBallKnockback);
            }

            if (Timer > BloodiancieDirector.BloodRainEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
