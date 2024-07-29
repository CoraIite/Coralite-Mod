using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstHalo_Reverse : Particle
    {
        public override string Texture => AssetDirectory.Particles + "IceHalo";

        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
            color = Color.White;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
        }

        public override void Update()
        {
            Rotation += 0.02f;
            Scale -= 0.26f;

            if (Scale < 0.2f)
                active = false;
        }

    }
}