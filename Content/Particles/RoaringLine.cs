using Coralite.Core;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Particles
{
    internal class RoaringLine : BasePRT
    {
        public override bool ShouldUpdatePosition() => false;

        public override string Texture => AssetDirectory.Particles + "Roaring";

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame = new Rectangle(0, 0, 128, 128);
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            Scale *= 1.1f;

            if (fadeIn > 15)
                Color *= 0.9f;

            fadeIn++;
            if (Color.A < 20)
                active = false;
        }

    }
}
