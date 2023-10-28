using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    public class BigFog : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Microsoft.Xna.Framework.Rectangle(0, 256 * Main.rand.Next(4), 256, 256);
        }

        public override void Update(Particle particle)
        {
            particle.velocity *= 0.98f;
            particle.rotation += 0.01f;
            particle.scale *= 0.997f;
            particle.color *= 0.94f;

            particle.fadeIn++;
            if (particle.fadeIn > 60 || particle.color.A < 10)
                particle.active = false;
        }

    }
}
