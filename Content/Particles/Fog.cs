using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Fog : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = new Rectangle(0, Main.rand.Next(4) * 64, 64, 64);
        }

        public override void Update(Particle particle)
        {
            particle.velocity *= 0.98f;
            particle.rotation += 0.01f;
            particle.scale *= 0.997f;
            particle.color *= 0.94f;

            particle.fadeIn++;
            if (particle.fadeIn > 120 || particle.color.A < 10)
                particle.active = false;
        }
    }
}
