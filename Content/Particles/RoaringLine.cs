using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    internal class RoaringLine : ModParticle
    {
        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override string Texture => AssetDirectory.Particles + "Roaring";

        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Microsoft.Xna.Framework.Rectangle(0, 0, 128, 128);
            particle.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void Update(Particle particle)
        {
            particle.scale *= 1.1f;

            if (particle.fadeIn > 15)
                particle.color *= 0.9f;

            particle.fadeIn++;
            if (particle.color.A < 20)
                particle.active = false;
        }

    }
}
