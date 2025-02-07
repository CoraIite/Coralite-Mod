using Coralite.Core;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstParticle : Particle
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
            if (Opacity % 2 == 0)
                Frame.Y = (int)(Opacity / 2) * 128;

            Opacity++;

            if (Opacity > 16)
                active = false;
        }
    }
}
