using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstParticle : ModParticle
    {
        public override string Texture => AssetDirectory.BabyIceDragon + "IceBurst";

        public override void OnSpawn(Particle particle)
        {
            particle.color = Color.White;
            particle.rotation = 0f;
            particle.frame = new Rectangle(0, 0, 128, 128);
            particle.shouldKilledOutScreen = false;
        }

        public override void Update(Particle particle)
        {
            if (particle.fadeIn % 2 == 0)
                particle.frame.Y = (int)(particle.fadeIn / 2) * 128;

            particle.fadeIn++;

            if (particle.fadeIn > 16)
                particle.active = false;
        }
    }
}
