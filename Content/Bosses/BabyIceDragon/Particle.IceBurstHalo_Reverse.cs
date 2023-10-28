using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstHalo_Reverse : ModParticle
    {
        public override string Texture => AssetDirectory.Particles + "IceHalo";

        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            particle.color = Color.White;
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = new Rectangle(0, 0, 128, 128);
        }

        public override void Update(Particle particle)
        {
            particle.rotation += 0.02f;
            particle.scale -= 0.26f;

            if (particle.scale < 0.2f)
                particle.active = false;
        }

    }
}