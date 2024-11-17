using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class SpeedLine : Particle
    {
        public override void SetProperty()
        {
            Frame = new Rectangle(0, 0, 64, 128);
            Rotation = Velocity.ToRotation() + 1.57f;
            oldPositions =
            [
                new Vector2(Scale,Scale*Main.rand.NextFloat(1,2))
            ];
        }

        public override void AI()
        {
            fadeIn++;
            Lighting.AddLight(Position, Color.ToVector3() * 0.3f);

            if (fadeIn > 8)
            {
                Velocity *= 0.96f;
                Color *= 0.8f;
            }

            if (fadeIn > 14)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            Rectangle frame = Frame;
            Vector2 origin = new(frame.Width / 2, frame.Height / 2);
            var pos = Position - Main.screenPosition;
            Color c = Color.White * 0.5f;
            if (oldPositions != null)
            {
                spriteBatch.Draw(mainTex, pos, frame, Color, Rotation, origin, oldPositions[0], SpriteEffects.None, 0f);
                spriteBatch.Draw(mainTex, pos, frame, c, Rotation, origin, oldPositions[0] * 0.5f, SpriteEffects.None, 0f);
            }
        }
    }
}
