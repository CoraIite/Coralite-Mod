using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class IceFog : ModParticle
    {
        public override void OnSpawn(Particle particle)
        {
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = new Rectangle(0, Main.rand.Next(4) * 64, 64, 64);
        }

        public override void Update(Particle particle)
        {
            particle.velocity *= 0.98f;
            particle.rotation += 0.01f;
            particle.scale *= 0.99f;
            particle.color *= 0.95f;

            particle.fadeIn++;
            if (particle.fadeIn > 30)
                particle.active = false;
        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
            Rectangle frame = particle.frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(modParticle.Texture2D.Value, particle.center - Main.screenPosition, frame, particle.color, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

        }
    }
}
