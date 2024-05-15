using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Particles
{
    public class RainbowHalo : Particle
    {
        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
            color = Color.White;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
        }

        public override void Update()
        {
            Rotation += 0.05f;
            if (fadeIn > 8)
                color *= 0.86f;

            Scale *= 1.04f;

            fadeIn++;
            if (fadeIn > 18)
                active = false;
        }
    }
}