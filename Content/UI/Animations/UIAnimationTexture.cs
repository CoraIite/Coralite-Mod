using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationTexture(string texturePath, Vector2 center) : UIAnimationComponent(center)
    {
        private readonly ATex tex = ModContent.Request<Texture2D>(texturePath);
        private Rectangle frameBox = new Rectangle(0, 0, 1, 1);
        private float scale = 1;
        private float scale2 = 1;

        public override float DeafaultDrawLayer => 3;

        public override void RecalculateOthers()
        {
            this.SetSize(tex.Frame(frameBox.Width, frameBox.Height, frameBox.X, frameBox.Y).Size());
        }

        public UIAnimationTexture SetFrameBox(Rectangle rect)
        {
            frameBox = rect;
            return this;
        }

        public UIAnimationTexture SetScale(float scale)
        {
            this.scale = scale;
            return this;
        }

        public override void DrawAnimation(SpriteBatch spriteBatch, int timer, Vector2 center, float fadeFactor)
        {
            Color c = DrawColor * fadeFactor;
            center += FadeOffset * (1 - fadeFactor);

            if (IsMouseHovering)
                scale2 = Helper.Lerp(scale2, 1.25f, 0.2f);
            else
                scale2 = Helper.Lerp(scale2, 1.05f, 0.2f);

            //tex.Value.QuickCenteredDraw(spriteBatch, center, c, Rotation);
            Rectangle sourceRectangle = tex.Frame(frameBox.Width, frameBox.Height, frameBox.X, frameBox.Y);
            spriteBatch.Draw(tex.Value, center, sourceRectangle, c, Rotation, sourceRectangle.Size() / 2, scale* scale2, 0, 0);

            //Helper.DrawDebugFrame(this, spriteBatch);
        }
    }
}
