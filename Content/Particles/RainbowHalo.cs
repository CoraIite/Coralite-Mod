using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Particles
{
    public class RainbowHalo : ModParticle
    {
        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            particle.color = Color.White;
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = new Rectangle(0, 0, 128, 128);
        }

        public override void Update(Particle particle)
        {
            particle.rotation += 0.05f;
            if (particle.fadeIn > 8)
                particle.color *= 0.86f;

            particle.scale *= 1.04f;

            particle.fadeIn++;
            if (particle.fadeIn > 18)
                particle.active = false;
        }
    }
}