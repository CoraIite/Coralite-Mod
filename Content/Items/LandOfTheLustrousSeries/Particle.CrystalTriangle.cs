using Coralite.Core;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class CrystalTriangle : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + "Triangle";

        private float FadeTime;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, Main.rand.Next(0, 5) * 64, 64, 64);
        }

        public override void AI()
        {
            Opacity++;

            if (Opacity > 5)
                Velocity *= 0.97f;

            if (Opacity > FadeTime)
            {
                Scale *= 0.95f;
                Color.A = (byte)(Color.A * 0.9f);
            }

            if (Opacity > 40 || Color.A < 10)
            {
                active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Rectangle frame = Frame;
            Vector2 origin = new(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, frame, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);
            return false;
        }

        public static CrystalTriangle Spawn(Vector2 center, Vector2 velocity, Color newColor, float fadeTime, float scale = 1)
        {
            CrystalTriangle c = PRTLoader.NewParticle<CrystalTriangle>(center, velocity, newColor, scale);
            c.FadeTime = fadeTime;
            return c;
        }
    }
}
