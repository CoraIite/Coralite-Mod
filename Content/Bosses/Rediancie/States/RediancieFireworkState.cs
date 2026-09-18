using Coralite.Content.Bosses.Rediancie.Core;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 赤玉烟花（大师二阶段远程）：慢速贴近，第 2 帧起盾、无敌 + 反弹并获得 3 发弹药（FTW 6）；弹药排成平面环，
    /// 49 帧后每 25 帧从末位弹药向四周射一轮 3~4 枚烟花（每轮消耗 1 弹药），265 帧收招；弹药耗尽立即收招并收盾。
    /// 旧 <c>Rediancie.Firework</c>（Rediancie.cs:508-562）。无敌 / 反弹原版不同步，改为每帧两端同声明。
    /// </summary>
    [VaultState((int)RediancieStateId.firework, typeof(RediancieContext))]
    internal sealed class RediancieFireworkState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.firework;

        private bool FireBeat => Timer >= RediancieDirector.FireworkWarmup && Timer % RediancieDirector.FireworkInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareChaseX(RediancieDirector.FireworkSpeedX, RediancieDirector.FireworkAccelX, RediancieDirector.FireworkTurnX, RediancieDirector.FireworkDamp);
            ctx.DeclareChaseYWithDeadZone(RediancieDirector.FireworkSpeedY, RediancieDirector.FireworkAccelY, RediancieDirector.FireworkTurnY, RediancieDirector.FireworkDamp);
            ctx.DeclareRotation(RediancieRotationMode.Normal);

            if (Timer == RediancieDirector.FireworkShieldFrame)
            {
                if (!Main.dedServ)
                {
                    RedShield.Spawn(ctx.Npc, RediancieDirector.FireworkShieldFrames);
                }

                ctx.SpawnFollowers(Main.getGoodWorld ? RediancieDirector.FireworkGainFollowersFtw : RediancieDirector.FireworkGainFollowers);
            }

            // 起盾后全程无敌 + 反弹（旧代码在第 2 帧置位、收招时解除）
            bool shielded = Timer >= RediancieDirector.FireworkShieldFrame;
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

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (ctx.FollowersEmpty)
            {
                return EndAttack(ctx);
            }

            if (FireBeat)
            {
                float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                int damage = RediancieDirector.FireworkDamage();
                int howMany = Main.getGoodWorld ? RediancieDirector.FireworkPerVolleyFtw : RediancieDirector.FireworkPerVolley;
                Vector2 anchor = ctx.Followers[^1].center;
                for (int i = 0; i < howMany; i++)
                {
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), anchor, rot.ToRotationVector2() * RediancieDirector.FireworkSpeed,
                        ModContent.ProjectileType<RedFirework>(), damage, RediancieDirector.FireworkKnockback, ctx.Npc.target,
                        0, RediancieDirector.FireworkLifeBase + (i * RediancieDirector.FireworkLifeStep));
                    rot += MathHelper.TwoPi / howMany;
                }

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > RediancieDirector.FireworkEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>收招收盾（两端各自；盾是本地粒子）。旧代码只在服务端弹药耗尽时收盾，客户端的盾会挂到自然消失。</summary>
        public override void OnExit(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            base.OnExit(machine, ctx);
            if (!Main.dedServ)
            {
                RedShield.HanderKill();
            }
        }
    }
}
