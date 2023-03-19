using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;

namespace Coralite.Content.Particles
{
    internal class RoaringLine : ModParticle
    {
        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override string Texture => AssetDirectory.Particles + "Roaring";

        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Microsoft.Xna.Framework.Rectangle(0, 0, 128, 128);
        }

        public override void Update(Particle particle)
        {
            particle.scale *= 1.04f;

            if (particle.fadeIn > 10f)
            {
                particle.color *= 0.97f;
            }

            particle.fadeIn++;
            if (particle.fadeIn > 40)
                particle.active = false;
        }

    }
}
