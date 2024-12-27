using Coralite.Core;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Particles
{
    public class BigFog : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public override void SetProperty()
        {
            Frame = new Rectangle(0, 256 * Main.rand.Next(4), 256, 256);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            Color *= 0.94f;

            Opacity++;
            if (Opacity > 60 || Color.A < 10)
                active = false;
        }

    }
}
