using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class SpeedLine : Particle
    {
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, 64, 128);
            Rotation = Velocity.ToRotation() + 1.57f;
            oldCenter =
            [
                new Vector2(Scale,Scale*Main.rand.NextFloat(1,2))
            ];
        }

        public override void Update()
        {
            fadeIn++;
            Lighting.AddLight(Center, color.ToVector3() * 0.3f);

            if (fadeIn > 8)
            {
                Velocity *= 0.96f;
                color *= 0.8f;
            }

            if (fadeIn > 14)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;
            Rectangle frame = Frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);
            var pos = Center - Main.screenPosition;
            Color c = color * 0.5f;
            if (oldCenter != null)
            {
                spriteBatch.Draw(mainTex, pos, frame, color, Rotation, origin, oldCenter[0], SpriteEffects.None, 0f);
                spriteBatch.Draw(mainTex, pos, frame, c, Rotation, origin, oldCenter[0] * 0.5f, SpriteEffects.None, 0f);
            }
        }
    }
}
