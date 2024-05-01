using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LightBall : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, 0, 128, 128);
            particle.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            particle.shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            particle.shader.UseSecondaryColor(Color.White);
        }

        public override void Update(Particle particle)
        {
            particle.fadeIn++;
            particle.shader.UseColor(particle.color);
            particle.shader.UseOpacity(0.65f);
            particle.shader.UseSaturation(2.1f);
            Lighting.AddLight(particle.center, particle.color.ToVector3() * 0.3f);

            if (particle.fadeIn < 10)
                particle.velocity *= 0.73f;
            else if (particle.fadeIn > 16)
            {
                particle.scale *= 0.92f;
                particle.color *= 0.92f;
                particle.velocity.Y -= 0.75f;
            }

            if (particle.fadeIn > 24)
                particle.active = false;
        }
    }
}
