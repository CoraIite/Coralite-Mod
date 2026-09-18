using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Content.Bosses.Rediancie;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 向上射击（远程基础招）：拉开 250 px 横向距离并把自己维持在玩家上方 250~400 px，弹药环逐渐张开；
    /// 30 帧后、200 帧前每 20 帧从随机一发弹药处向上打出一簇赤玉，每簇消耗 1 弹药，260 帧收招；弹药耗尽立即收招。
    /// 旧 <c>Bloodiancie.UpShoot</c>（AI.cs:571-626）。Y 轴由本状态自管（两端同算），发射音效旧代码写在服务端分支里，改到有画面的一端放。
    /// </summary>
    [VaultState((int)BloodiancieStateId.upShoot, typeof(BloodiancieContext))]
    internal sealed class BloodiancieUpShootState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.upShoot;

        /// <summary>出手拍：30 帧起、200 帧前每 20 帧（两端同算）。</summary>
        private bool FireBeat => Timer >= BloodiancieDirector.UpShootWarmup
            && Timer < BloodiancieDirector.UpShootFireEnd
            && Timer % BloodiancieDirector.UpShootInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            ctx.DeclareChaseXBeyond(BloodiancieDirector.UpShootKeepX, BloodiancieDirector.UpShootSpeedX,
                BloodiancieDirector.UpShootAccelX, BloodiancieDirector.UpShootTurnX, BloodiancieDirector.UpShootDampX);
            ctx.DeclareRotation(BloodiancieRotationMode.Normal);

            // Y 自管：够不着高度就上浮、太高了就回落（旧代码的回落钳位等于一帧拍到 +6，原样保留）
            float yLength = ctx.Npc.Center.Y - ctx.Target.Center.Y;
            if (yLength > BloodiancieDirector.UpShootRiseBelow)
            {
                ctx.Npc.velocity.Y -= BloodiancieDirector.UpShootRiseAccel;
                if (ctx.Npc.velocity.Y < BloodiancieDirector.UpShootRiseLimit)
                {
                    ctx.Npc.velocity.Y = BloodiancieDirector.UpShootRiseLimit;
                }
            }
            else if (yLength < BloodiancieDirector.UpShootFallAbove)
            {
                ctx.Npc.velocity.Y += BloodiancieDirector.UpShootFallAccel;
                if (ctx.Npc.velocity.Y < BloodiancieDirector.UpShootFallLimit)
                {
                    ctx.Npc.velocity.Y = BloodiancieDirector.UpShootFallLimit;
                }
            }
            else
            {
                ctx.Npc.velocity.Y *= BloodiancieDirector.UpShootSettleDampY;
            }

            if (ctx.FollowersEmpty)
            {
                return;
            }

            ctx.UpdateFollowersUpShoot(Timer);

            if (FireBeat && !Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.Item5, ctx.Npc.Center);
            }
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (ctx.FollowersEmpty)
            {
                return EndAttack(ctx);
            }

            if (FireBeat)
            {
                int index = Main.rand.Next(ctx.Followers.Count);
                int damage = BloodiancieDirector.UpShootDamage();
                int shootCount = BloodiancieDirector.UpShootCount();
                Vector2 position = ctx.Followers[index].center;
                for (int i = 0; i < shootCount; i++)
                {
                    Vector2 velocity = new Vector2(0, -BloodiancieDirector.UpShootSpeed)
                        .RotatedBy(Main.rand.NextFloat(-BloodiancieDirector.UpShootSpread, BloodiancieDirector.UpShootSpread));
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), position, velocity,
                        ModContent.ProjectileType<Rediancie_Strike>(), damage, BloodiancieDirector.UpShootKnockback, ctx.Npc.target);
                }

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > BloodiancieDirector.UpShootEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
