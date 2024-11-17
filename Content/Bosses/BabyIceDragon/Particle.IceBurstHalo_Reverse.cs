using Coralite.Core;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurstHalo_Reverse : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + "IceHalo";

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Color = Color.White;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Rotation += 0.02f;
            Scale -= 0.26f;

            if (Scale < 0.2f)
                active = false;
        }

    }
}