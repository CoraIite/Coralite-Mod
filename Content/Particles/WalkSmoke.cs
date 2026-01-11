using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class WalkSmoke : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public int direction;
        public float alpha = 1;
        public Vector2 scale2 = Vector2.One;

        public bool addDraw = false;
        public float addAlpha = 1;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity > 0 && Opacity % 2 == 0)
            {
                Frame.Y++;
                if (Frame.Y > 6)
                {
                    active = false;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 scale = Scale * scale2;
            Rectangle frame = TexValue.Frame(1, 7, 0, Frame.Y);
            Vector2 pos = Position - Main.screenPosition + new Vector2(0, frame.Height / 6 * scale.Y);
            Color c = Lighting.GetColor(Position.ToTileCoordinates(), Color) * alpha;

            SpriteEffects effects = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new(direction > 0 ? 0 : frame.Width, frame.Height);
            float rotation = Rotation + (direction > 0 ? 0 : MathHelper.Pi);

            spriteBatch.Draw(TexValue, pos, frame, c, rotation, origin, scale, effects, 0);

            if (addDraw)
            {
                c.A = 0;
                c *= addAlpha;
                spriteBatch.Draw(TexValue, pos, frame, c, rotation, origin, scale, effects, 0);
            }


            return false;
        }
    }
}
