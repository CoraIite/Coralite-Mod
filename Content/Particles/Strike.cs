using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Strike : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, 0, 128, 128);
            particle.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "StarsDustPass");
            particle.shader.UseSecondaryColor(Color.White);
        }

        public override void Update(Particle particle)
        {
            particle.shader.UseColor(particle.color);

            if (particle.fadeIn % 2 == 0)
                particle.frame.Y = (int)(particle.fadeIn / 2) * 128;

            float factor = particle.fadeIn / 14;

            particle.shader.UseOpacity(0.5f + factor * 0.4f);
            particle.shader.UseSaturation(2.3f - factor * 0.8f);

            particle.fadeIn++;

            if (particle.fadeIn > 16)
                particle.active = false;
        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
            Vector2 origin = new Vector2(96, 96);

            spriteBatch.Draw(modParticle.Texture2D.Value, particle.center - Main.screenPosition, particle.frame, particle.color, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

        }
    }
}
