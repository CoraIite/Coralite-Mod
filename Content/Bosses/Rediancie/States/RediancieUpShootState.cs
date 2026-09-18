using Coralite.Content.Bosses.Rediancie.Core;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 赤玉雨（远程）：悬停在玩家上方，弹药环逐渐张开，30 帧后每 40 帧从随机一发弹药处向上射 2~4 枚赤玉（每轮消耗 1 弹药），260 帧收招；
    /// 弹药耗尽立即收招。旧 <c>Rediancie.UpShoot</c>（Rediancie.cs:762-801）。出膛弹药下标在权威端掷骰，客户端只放音效。
    /// </summary>
    [VaultState((int)RediancieStateId.upShoot, typeof(RediancieContext))]
    internal sealed class RediancieUpShootState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.upShoot;

        private bool FireBeat => Timer >= RediancieDirector.UpShootWarmup && Timer % RediancieDirector.UpShootInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareHoverAboveTarget();
            ctx.DeclareRotation(RediancieRotationMode.Normal);

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

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (ctx.FollowersEmpty)
            {
                return EndAttack(ctx);
            }

            if (FireBeat)
            {
                int index = Main.rand.Next(ctx.Followers.Count);
                int damage = RediancieDirector.UpShootDamage();
                int shootCount = RediancieDirector.UpShootCount();
                for (int i = 0; i < shootCount; i++)
                {
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), ctx.Followers[index].center,
                        new Vector2(0, -RediancieDirector.UpShootSpeed).RotatedBy(Main.rand.NextFloat(-RediancieDirector.UpShootSpread, RediancieDirector.UpShootSpread)),
                        ModContent.ProjectileType<Rediancie_Strike>(), damage, RediancieDirector.UpShootKnockback);
                }

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > RediancieDirector.UpShootEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
