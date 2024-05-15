using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstHalo : Particle
    {
        public override string Texture => AssetDirectory.Particles + "IceHalo";

        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
            Scale = 0.02f;
            color = Color.White;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
            shouldKilledOutScreen = false;
        }

        public override void Update()
        {
            if (fadeIn > 14)
                color *= 0.88f;

            Scale += 0.875f;

            fadeIn++;
            if (fadeIn > 20)
                active = false;
        }
    }
}
