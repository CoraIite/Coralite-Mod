using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Core;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 射出炸弹（远程）：贴近玩家蓄力 60 帧，70 帧一次性甩出 7 枚扇形炸弹（飞行时间与初速随机、落点罩住玩家，引爆倒计时 2~8 秒），
    /// 每枚消耗 1 发弹药，弹药耗尽就少甩几枚（第一枚永远会出），120 帧收招。旧 <c>Bloodiancie.ShootBomb</c>（AI.cs:628-684）。
    /// 旧代码一帧内把甩出音效放了 7 遍（炸弹数量还随弹药变化），改成开火帧在有画面的一端放一次。
    /// </summary>
    [VaultState((int)BloodiancieStateId.shootBomb, typeof(BloodiancieContext))]
    internal sealed class BloodiancieShootBombState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.shootBomb;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer < BloodiancieDirector.BombChaseFrames)
            {
                ctx.DeclareChaseXBeyond(BloodiancieDirector.ChaseDeadZone, BloodiancieDirector.BombSpeedX,
                    BloodiancieDirector.BombAccelX, BloodiancieDirector.BombTurnX, BloodiancieDirector.BombDamp);
                ctx.DeclareChaseYWithDeadZone(BloodiancieDirector.BombSpeedY, BloodiancieDirector.BombAccelY,
                    BloodiancieDirector.BombTurnY, BloodiancieDirector.BombDamp);

                if (Timer % BloodiancieDirector.BombDustInterval == 0)
                {
                    ChargeDust(ctx, Timer / BloodiancieDirector.BombDustPerFrames, BloodiancieDirector.ChargeDustScaleGain);
                }
            }
            else
            {
                ctx.DeclareDamp(BloodiancieDirector.BombSettleDamp);
            }

            if (Timer == BloodiancieDirector.BombFireFrame && !Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, ctx.Npc.Center);
            }

            ctx.DeclareRotation(BloodiancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer == BloodiancieDirector.BombFireFrame)
            {
                // 含 Main.rand 的飞行时间 / 初速 / 引信整体留在权威端，弹幕本身由 NewProjectile 自动同步
                Vector2 targetVec = ctx.Target.Center - ctx.Npc.Center;
                Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);
                float length = targetVec.Length();

                for (int i = BloodiancieDirector.BombFanFrom; i < BloodiancieDirector.BombFanTo; i++)
                {
                    int shootTime = Main.rand.Next(BloodiancieDirector.BombFlightMin, BloodiancieDirector.BombFlightMax);
                    float velLength = length * Main.rand.NextFloat(BloodiancieDirector.BombSpeedScaleMin, BloodiancieDirector.BombSpeedScaleMax) / shootTime;
                    if (velLength > BloodiancieDirector.BombSpeedCap)
                    {
                        velLength = BloodiancieDirector.BombSpeedCap;
                    }

                    Projectile.NewProjectile(ctx.Npc.GetSource_FromAI(), ctx.Npc.Center,
                        dir.RotatedBy(i * BloodiancieDirector.BombFanStep) * velLength,
                        ModContent.ProjectileType<RedBomb>(), 1, BloodiancieDirector.BombKnockback,
                        ai0: shootTime, ai2: Main.rand.Next(BloodiancieDirector.BombFuseMin, BloodiancieDirector.BombFuseMax));

                    // 第一枚永远会出：旧代码先生成再判弹药，弹药耗尽才停下
                    if (!ctx.DespawnFollowers(1))
                    {
                        break;
                    }
                }
            }

            if (Timer > BloodiancieDirector.BombEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
