using Coralite.Core;
using Terraria;

namespace Coralite.Content.Particles
{
    public class SnowFlower : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame = new Rectangle(0, Main.rand.Next(3) * 64, 64, 64);
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            Rotation += 0.02f;
            Scale *= 0.955f;
            Color *= 0.97f;
            Opacity++;

            if (Opacity > 24)
                active = false;
        }
    }
}
