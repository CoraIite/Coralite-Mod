using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.UI;

namespace Coralite.Content.UI.BookUI
{
    public abstract class PageJumpButton : UIElement
    {
        protected Asset<Texture2D> texture;
        protected Asset<Texture2D> borderTexture;

        protected float scaleActive = 1.2f;

        protected float scaleInactive = 1f;

        public abstract int PageToJump { get; }

        public void SetSize(float width, float height)
        {
            Width.Set(width, 0);
            Height.Set(height, 0);
        }

        public void SetHoverImage(Asset<Texture2D> texture)
        {
            borderTexture = texture;
        }

        public void SetImage(Asset<Texture2D> tex)
        {
            texture = tex;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(CoraliteSoundID.MenuTick);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }

        public float GetScale()
        {
            return IsMouseHovering ? scaleActive : scaleInactive;
        }
    }
}
