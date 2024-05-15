using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LightBall : Particle
    {
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, 128, 128);
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            shader.UseSecondaryColor(Color.White);
        }

        public override void Update()
        {
            fadeIn++;
            shader.UseColor(color);
            shader.UseOpacity(0.65f);
            shader.UseSaturation(2.1f);
            Lighting.AddLight(Center, color.ToVector3() * 0.3f);

            if (fadeIn < 10)
                Velocity *= 0.73f;
            else if (fadeIn > 16)
            {
                Scale *= 0.92f;
                color *= 0.92f;
                Velocity.Y -= 0.75f;
            }

            if (fadeIn > 24)
                active = false;
        }
    }
}
