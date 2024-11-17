using Coralite.Core;
using InnoVault.PRT;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstParticle : BasePRT
    {
        public override string Texture => AssetDirectory.BabyIceDragon + "IceBurst";

        public override void SetProperty()
        {
            Color = Color.White;
            Rotation = 0f;
            Frame = new Rectangle(0, 0, 128, 128);
            ShouldKillWhenOffScreen = false;
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            if (fadeIn % 2 == 0)
                Frame.Y = (int)(fadeIn / 2) * 128;

            fadeIn++;

            if (fadeIn > 16)
                active = false;
        }
    }
}
