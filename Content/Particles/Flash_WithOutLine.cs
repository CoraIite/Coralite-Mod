using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Flash_WithOutLine : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.velocity *= 0f;
            particle.rotation =Main.rand.NextFloat(6.282f);
            particle.frame = new Rectangle(0, 0, 128, 128);
            particle.fadeIn = 0;
            particle.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "StarsDustPass");
            particle.shader.UseSecondaryColor(Color.White);
        }

        public override void Update(Particle particle)
        {
            //particle.color *= 0.98f;
            particle.shader.UseColor(particle.color);
            float factor = particle.fadeIn / 16;
            particle.shader.UseOpacity(0.55f + factor * 0.1f);
            particle.shader.UseSaturation(2.5f - factor * 0.7f);

            if (particle.fadeIn % 2 == 0)
                particle.frame.Y = (int)(particle.fadeIn / 2) * 128;

            Lighting.AddLight(particle.center, particle.color.ToVector3());

            particle.fadeIn++;

            if (particle.fadeIn > 16)
                particle.active = false;
        }
    }
}
