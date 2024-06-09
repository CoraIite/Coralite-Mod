﻿using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleMagicBall : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.FrostStaff);
            dust.noGravity = true;

            Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 0.1f,
                    CoraliteContent.ParticleType<Fog>(), Color.AliceBlue, Main.rand.NextFloat(0.6f, 0.8f));

            if (Projectile.timeLeft % 2 == 0)
            {
                Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(16, 16), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * 0.1f,
                    CoraliteContent.ParticleType<SnowFlower>(), Color.White, Main.rand.NextFloat(0.2f, 0.3f));
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                for (int i = 0; i < 4; i++)
                    Particle.NewParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) * Main.rand.NextFloat(0.05f, 0.2f),
                        CoraliteContent.ParticleType<SnowFlower>(), Color.White, Main.rand.NextFloat(0.4f, 0.6f));

                Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceHalo>(), Color.White, 0.2f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //生成3个冰锥弹幕
            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(140, 220)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                    Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(24, 24) - center).SafeNormalize(Vector2.UnitY) * 12;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, velocity,
                        ModContent.ProjectileType<IcicleFalling>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //生成3个冰锥弹幕
            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(140, 220)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, Vector2.One,
                        ModContent.ProjectileType<IcicleFalling>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner,
                        Projectile.Center.X + Main.rand.Next(-24, 24), Projectile.Center.Y);
                }
        }
    }
}
