using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class FireParticle : ModParticle
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public override void OnSpawn(Particle particle)
        {
            //particle.color.A = 0;

            particle.frame = new Rectangle(0, Main.rand.Next(3), 1, 1);

            particle.oldRot = new float[1]
            {
                Main.rand.Next(2)
            };

            particle.rotation = particle.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Update(Particle particle)
        {
            particle.fadeIn++;

            if (particle.fadeIn % 5 == 0)
            {
                particle.frame.Y++;
                if (particle.frame.Y > 15)
                    particle.active = false;
            }

            particle.velocity *= 0.95f;
            particle.color *= 0.96f;
        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
            Rectangle frame = modParticle.Texture2D.Frame(1, 16, 0, particle.frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(modParticle.Texture2D.Value, particle.center - Main.screenPosition, frame, particle.color, particle.rotation, origin, particle.scale
                , particle.oldRot[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(modParticle.Texture2D.Value, particle.center - Main.screenPosition, frame, particle.color * 0.5f, particle.rotation, origin, particle.scale
                , particle.oldRot[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
