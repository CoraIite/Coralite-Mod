using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstHalo : ModParticle
    {
        public override string Texture => AssetDirectory.Particles + "IceHalo";

        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            particle.scale = 0.02f;
            particle.color = Color.White;
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = new Rectangle(0, 0, 128, 128);
            particle.shouldKilledOutScreen = false;
        }

        public override void Update(Particle particle)
        {
            if (particle.fadeIn > 14)
                particle.color *= 0.88f;

            particle.scale += 0.875f;

            particle.fadeIn++;
            if (particle.fadeIn > 20)
                particle.active = false;
        }
    }
}
