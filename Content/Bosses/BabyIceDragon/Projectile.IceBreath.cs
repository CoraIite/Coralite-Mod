using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBreath : ModProjectile
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.timeLeft = 80;
            Projectile.aiStyle = -1;
            Projectile.maxPenetrate = -1;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y -= 0.08f;
            if (Projectile.velocity.Y < -16)
                Projectile.velocity.Y = -16;

            if (Projectile.timeLeft < 40)
                Projectile.velocity.X *= 0.98f;
            if (Projectile.timeLeft % 3 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.FrostStaff);
                dust.noGravity = true;

                Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(16, 16), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * 0.1f,
                    CoraliteContent.ParticleType<SnowFlower>(), Color.White, Main.rand.NextFloat(0.3f, 0.4f));
            }

            Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(16, 16), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 0.2f - Vector2.UnitY * 2,
                   CoraliteContent.ParticleType<Fog>(), Color.AliceBlue, Main.rand.NextFloat(0.8f, 0.9f));
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}