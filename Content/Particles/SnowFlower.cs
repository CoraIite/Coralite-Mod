using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Particles
{
    public class SnowFlower : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, Main.rand.Next(3) * 64, 64, 64);
            particle.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void Update(Particle particle)
        {
            particle.rotation += 0.02f;
            particle.scale *= 0.96f;
            particle.color *= 0.975f;
            particle.fadeIn++;

            if (particle.fadeIn > 20)
                particle.active = false;
        }
    }
}
