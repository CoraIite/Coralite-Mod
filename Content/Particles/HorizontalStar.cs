using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class HorizontalStar : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, 0, 126, 93);
            particle.fadeIn = 0;
            particle.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "StarsDustPass");
        }

        public override void Update(Particle particle)
        {
            particle.shader.UseColor(particle.color);

            if (particle.fadeIn < 10)
            {
                particle.scale *= 1.06f;
                float factor = particle.fadeIn / 10;
                particle.shader.UseOpacity(0.65f - factor * 0.25f);
                particle.shader.UseSaturation(1.5f + factor * 0.6f);
            }
            else
            {
                particle.velocity *= 0.9f;
                particle.scale *= 0.9f;
                float factor = (particle.fadeIn - 10) / 10;
                particle.shader.UseOpacity(0.4f + factor * 0.3f);
                particle.shader.UseSaturation(2.3f - factor * 0.8f);
            }

            Lighting.AddLight(particle.center, particle.color.ToVector3());

            particle.fadeIn++;

            if (particle.fadeIn > 23)
                particle.active = false;
        }
    }
}
