using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Fog : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, Main.rand.Next(4) * 64, 64, 64);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            color *= 0.94f;

            fadeIn++;
            if (fadeIn > 120 || color.A < 10)
                active = false;
        }
    }
}
