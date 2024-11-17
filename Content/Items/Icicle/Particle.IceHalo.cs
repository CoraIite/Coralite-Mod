using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Items.Icicle
{
    /// <summary>
    /// 冰冻光环
    /// </summary>
    public class IceHalo : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + Name;
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
            Rotation += 0.05f;
            if (fadeIn > 8)
                Color *= 0.92f;

            Scale *= 1.07f;

            fadeIn++;
            if (fadeIn > 18)
                active = false;
        }
    }
}
