using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Content.Bosses.Rediancie;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 赤玉烟花（大师二阶段远程）：慢速贴近，第 2 帧起盾、无敌 + 反弹并获得 3 发弹药（FTW 6）；弹药排成平面环，
    /// 49 帧后每 25 帧从末位弹药向四周射一轮 3~5 枚烟花（每轮消耗 1 弹药），265 帧收招；弹药耗尽立即收招并收盾。
    /// 旧 <c>Bloodiancie.Firework</c>（AI.cs:309-363）。无敌 / 反弹原版不同步，改为每帧两端同声明；
    /// 发射音效旧代码写在服务端分支里（客户端根本听不到），改到有画面的一端放。
    /// </summary>
    [VaultState((int)BloodiancieStateId.firework, typeof(BloodiancieContext))]
    internal sealed class BloodiancieFireworkState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.firework;

        private bool FireBeat => Timer >= BloodiancieDirector.FireworkWarmup && Timer % BloodiancieDirector.FireworkInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            ctx.DeclareChaseX(BloodiancieDirector.FireworkSpeedX, BloodiancieDirector.FireworkAccelX,
                BloodiancieDirector.FireworkTurnX, BloodiancieDirector.FireworkDamp);
            ctx.DeclareChaseYWithDeadZone(BloodiancieDirector.FireworkSpeedY, BloodiancieDirector.FireworkAccelY,
                BloodiancieDirector.FireworkTurnY, BloodiancieDirector.FireworkDamp);
            ctx.DeclareRotation(BloodiancieRotationMode.Normal);

            if (Timer == BloodiancieDirector.FireworkShieldFrame)
            {
                if (!Main.dedServ)
                {
                    RedShield.Spawn(ctx.Npc, BloodiancieDirector.FireworkShieldFrames);
                }

                ctx.SpawnFollowers(Main.getGoodWorld ? BloodiancieDirector.FireworkGainFollowersFtw : BloodiancieDirector.FireworkGainFollowers);
            }

            // 起盾后全程无敌 + 反弹（旧代码在第 2 帧置位、收招时解除）
            bool shielded = Timer >= BloodiancieDirector.FireworkShieldFrame;
            ctx.Invulnerable = shielded;
            ctx.ReflectsProjectiles = shielded;

            if (ctx.FollowersEmpty)
            {
                return;
            }

            ctx.UpdateFollowersFirework(Timer);

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
                float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                int damage = BloodiancieDirector.FireworkDamage();
                int howMany = Main.rand.Next(BloodiancieDirector.FireworkVolleyMin, BloodiancieDirector.FireworkVolleyMax);
                Vector2 anchor = ctx.Followers[^1].center;
                for (int i = 0; i < howMany; i++)
                {
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), anchor, rot.ToRotationVector2() * BloodiancieDirector.FireworkSpeed,
                        ModContent.ProjectileType<RedFirework>(), damage, BloodiancieDirector.FireworkKnockback, ctx.Npc.target,
                        0, BloodiancieDirector.FireworkLifeBase + (i * BloodiancieDirector.FireworkLifeStep));
                    rot += MathHelper.TwoPi / howMany;
                }

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > BloodiancieDirector.FireworkEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>收招收盾（两端各自；盾是本地粒子）。旧代码只在服务端弹药耗尽时收盾，客户端的盾会挂到自然消失。</summary>
        public override void OnExit(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            base.OnExit(machine, ctx);
            if (!Main.dedServ)
            {
                RedShield.HanderKill();
            }
        }
    }
}
