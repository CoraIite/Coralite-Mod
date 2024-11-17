using Coralite.Core.Systems.ParticleSystem;
using Terraria;

namespace Coralite.Content.Items.Icicle
{
    /// <summary>
    /// 冰冻光环
    /// </summary>
    public class IceHalo : Particle
    {
        public override bool ShouldUpdateCenter() => false;

        public override void SetProperty()
        {
            color = Color.White;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
        }

        public override void AI()
        {
            Rotation += 0.05f;
            if (fadeIn > 8)
                color *= 0.92f;

            Scale *= 1.07f;

            fadeIn++;
            if (fadeIn > 18)
                active = false;
        }
    }
}
