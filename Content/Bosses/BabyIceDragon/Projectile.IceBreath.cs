using Terraria.ModLoader;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;
using Coralite.Content.Particles;

namespace Coralite.Content.Bosses.BabyIceDragon
{
   public class IceBreath : ModProjectile
   {
      public override string Texture => AssetDirectory.DefaultItem;

      public override void SetDefaults()
      {
         Projectile.width = Projectile.height = 64;
         Projectile.timeLeft = 180;
         Projectile.aiStyle = -1;
         Projectile.maxPenetrate = -1;

         Projectile.hostile = true;
         Projectile.tileCollide = true;
      }

      public override void AI()
      {
         Projectile.velocity.Y -= 0.02f;
         if (Projectile.velocity.Y < -4)
            Projectile.velocity.Y = -4;

         if (Projectile.timeLeft < 80)
            Projectile.velocity.X *= 0.99f;

         Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(48, 48), -Vector2.UnitY,
            CoraliteContent.ParticleType<IceFog>(), Color.AliceBlue, Main.rand.NextFloat(0.6f, 0.8f));
      }

      public override bool PreDraw(ref Color lightColor) => false;
   }
}