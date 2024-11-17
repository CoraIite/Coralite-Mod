using Coralite.Core;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LightBall : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public override void SetProperty()
        {
            Frame = new Rectangle(0, 0, 128, 128);
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            shader.UseSecondaryColor(Color.White);
        }

        public override void AI()
        {
            fadeIn++;
            shader.UseColor(Color);
            shader.UseOpacity(0.65f);
            shader.UseSaturation(2.1f);
            Lighting.AddLight(Position, Color.ToVector3() * 0.3f);

            if (fadeIn < 10)
                Velocity *= 0.73f;
            else if (fadeIn > 16)
            {
                Scale *= 0.92f;
                Color *= 0.92f;
                Velocity.Y -= 0.75f;
            }

            if (fadeIn > 24)
                active = false;
        }
    }
}
