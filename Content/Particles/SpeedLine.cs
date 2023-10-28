using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class SpeedLine : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, 0, 64, 128);
            particle.rotation = particle.velocity.ToRotation() + 1.57f;
            particle.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "StarsDustPass");
            particle.shader.UseSecondaryColor(Color.White * 0.75f);
            particle.oldCenter = new Vector2[1]
            {
                new Vector2(particle.scale,particle.scale*Main.rand.NextFloat(1,2))
            };
        }

        public override void Update(Particle particle)
        {
            particle.fadeIn++;
            particle.shader.UseColor(particle.color);
            particle.shader.UseOpacity(0.65f);
            particle.shader.UseSaturation(2.1f);
            Lighting.AddLight(particle.center, particle.color.ToVector3() * 0.3f);

            if (particle.fadeIn > 8)
            {
                float factor = 1 - (particle.fadeIn - 6) / 6;
                particle.velocity *= 0.96f;
                particle.shader.UseSecondaryColor(Color.White * 0.75f * factor);
            }

            if (particle.fadeIn > 14)
                particle.active = false;
        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
            Rectangle frame = particle.frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

            if (particle.oldCenter != null)
                spriteBatch.Draw(modParticle.Texture2D.Value, particle.center - Main.screenPosition, frame, particle.color, particle.rotation, origin, particle.oldCenter[0], SpriteEffects.None, 0f);
        }
    }
}
