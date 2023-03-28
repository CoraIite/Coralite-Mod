using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurst : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

        public ref float Length => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1920;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 20;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 10 && Projectile.timeLeft % 4 == 0)
                for (int i = 0; i < 3; i++)
                    Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo>(), Color.White, 0.25f);

            Length += 96f;
            Projectile.netUpdate = true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return Vector2.Distance(Projectile.Center, target.Center) < Length;
        }

        public override bool PreDraw(ref Color lightColor) => false;

    }
}
