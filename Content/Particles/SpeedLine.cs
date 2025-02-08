using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class SpeedLine : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame = new Rectangle(0, 0, 64, 128);
            Rotation = Velocity.ToRotation() + 1.57f;
            oldPositions =
            [
                new Vector2(Scale,Scale*Main.rand.NextFloat(1,2))
            ];
        }

        public override void AI()
        {
            Opacity++;
            Lighting.AddLight(Position, Color.ToVector3() * 0.3f);

            if (Opacity > 8)
            {
                Velocity *= 0.96f;
                Color *= 0.8f;
            }

            if (Opacity > 14 || Color.A < 10)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
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

            return false;
        }
    }

    public class LaserLine : Particle
    {
        public override string Texture => AssetDirectory.Particles + "SpeedLine";

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame = new Rectangle(0, 0, 64, 128);
            Rotation = Velocity.ToRotation() + 1.57f;
            oldPositions =
            [
                new Vector2(Scale,Scale*Main.rand.NextFloat(1,2))
            ];
        }

        public override void AI()
        {
            Opacity++;
            Lighting.AddLight(Position, Color.ToVector3() * 0.2f);

            if (Opacity > 5)
            {
                Velocity *= 0.96f;
                Color.A = (byte)(0.75f * Color.A);

                if (Opacity > 7)
                {
                    if (oldPositions != null)
                    {
                        oldPositions[0].X *= 0.95f;
                    }
                }
            }

            if (Opacity > 14 || Color.A < 10)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
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

            return false;
        }
    }
}
