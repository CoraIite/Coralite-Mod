using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationTexture(string texturePath, Vector2 center) : UIAnimationComponent(center)
    {
        private readonly ATex tex = ModContent.Request<Texture2D>(texturePath);

        public override void RecalculateOthers()
        {
            this.SetSize(tex.Size());
        }

        public override void DrawAnimation(SpriteBatch spriteBatch, int timer, Vector2 center, float fadeFactor)
        {
            Color c = DrawColor * fadeFactor;
            center += FadeOffset * (1 - fadeFactor);

            tex.Value.QuickCenteredDraw(spriteBatch, center, c, Rotation);
        }
    }
}
