using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    public class SnowFlower : Particle
    {
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, Main.rand.Next(3) * 64, 64, 64);
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void Update()
        {
            Rotation += 0.02f;
            Scale *= 0.955f;
            color *= 0.97f;
            fadeIn++;

            if (fadeIn > 24)
                active = false;
        }
    }
}
