using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class FireParticle : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + Name;

        private SpriteEffects effect;
        public int MaxFrameCount = 5;

        public override void SetProperty()
        {
            //color.A = 0;

            Frame = new Rectangle(0, Main.rand.Next(3), 1, 1);
            effect = Main.rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rotation = Velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void AI()
        {
            fadeIn++;

            if (fadeIn % MaxFrameCount == 0)
            {
                Frame.Y++;
                if (Frame.Y > 15)
                    active = false;
            }

            Velocity *= 0.95f;
            Color *= 0.96f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            Rectangle frame = mainTex.Frame(1, 16, 0, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Color, Rotation, origin, Scale
                , effect, 0f);
            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Color * 0.5f, Rotation, origin, Scale
                , effect, 0f);

            return false;
        }

        public override void DrawInUI(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            Rectangle frame = mainTex.Frame(1, 16, 0, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position, frame, Color, Rotation, origin, Scale
                , effect, 0f);
            spriteBatch.Draw(mainTex, Position, frame, Color * 0.5f, Rotation, origin, Scale
                , effect, 0f);
        }
    }
}
