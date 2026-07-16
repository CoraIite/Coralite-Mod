using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationItem(int itemType, Vector2 center) : UIAnimationComponent(center)
    {
        private float scale = 1;
        private float scale2 = 1;

        public override void RecalculateOthers()
        {
            Helper.GetItemTexAndFrame(itemType, out _, out Rectangle frameBox);
            HoverItemType = itemType;
            this.SetSize(frameBox.Size());
        }

        public UIAnimationItem SetScale(float scale)
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
            MagikeHelper.DrawItem(spriteBatch, ContentSamples.ItemsByType[itemType], center, int.MaxValue, c, scale * scale2);
        }
    }
}
