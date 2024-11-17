using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Sparkle_Big : Particle
    {
        public override void SetProperty()
        {
            Velocity *= 0f;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 128, 128);
            fadeIn = 0;
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            shader.UseSecondaryColor(Color.White);
        }

        public override void AI()
        {
            //color *= 0.98f;
            shader.UseColor(Color);
            float factor = fadeIn / 16;
            shader.UseOpacity(0.55f + (factor * 0.1f));
            shader.UseSaturation(2.5f - (factor * 0.7f));

            if (fadeIn % 2 == 0)
                Frame.Y = (int)(fadeIn / 2) * 128;

            Lighting.AddLight(Position, Color.ToVector3());

            fadeIn++;

            if (fadeIn > 16)
                active = false;
        }
    }
}
