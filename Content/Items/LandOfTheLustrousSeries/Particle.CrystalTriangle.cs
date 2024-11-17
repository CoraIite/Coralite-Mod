using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class CrystalTriangle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "Triangle";

        private float FadeTime;

        public override void SetProperty()
        {
            drawNonPremultiplied = true;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, Main.rand.Next(0, 5) * 64, 64, 64);
        }

        public override void AI()
        {
            fadeIn++;

            if (fadeIn > 5)
                Velocity *= 0.97f;

            if (fadeIn > FadeTime)
            {
                Scale *= 0.95f;
                color.A = (byte)(color.A * 0.9f);
            }

            if (fadeIn > 40 || color.A < 10)
            {
                active = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

        public static CrystalTriangle Spawn(Vector2 center, Vector2 velocity, Color newColor, float fadeTime, float scale = 1)
        {
            CrystalTriangle c = NewParticle<CrystalTriangle>(center, velocity, newColor, scale);
            c.FadeTime = fadeTime;
            return c;
        }

        public override void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Rectangle frame = Frame;
            Vector2 origin = new(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
