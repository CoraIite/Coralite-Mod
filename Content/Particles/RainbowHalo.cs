using Coralite.Core;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Particles
{
    public class RainbowHalo : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Color = Color.White;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
        }

        public override void AI()
        {
            Rotation += 0.05f;
            if (fadeIn > 8)
                Color *= 0.86f;

            Scale *= 1.04f;

            fadeIn++;
            if (fadeIn > 18)
                active = false;
        }
    }
}