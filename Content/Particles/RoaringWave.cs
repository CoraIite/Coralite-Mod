using Coralite.Core;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Particles
{
    public class RoaringWave : Particle
    {
        public override bool ShouldUpdatePosition() => false;

        public override string Texture => AssetDirectory.Particles + "Roaring";

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame = new Rectangle(0, 128, 128, 128);
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            Scale *= 1.1f;

            if (Opacity > 15)
                Color *= 0.9f;

            Opacity++;
            if (Color.A < 20)
                active = false;
        }
    }
}
