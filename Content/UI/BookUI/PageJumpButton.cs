using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.UI;

namespace Coralite.Content.UI.BookUI
{
    public abstract class PageJumpButton(Func<UI_BookPanel> GetBookPanel, Func<int> GetPage) : UIElement
    {
        protected ATex texture;
        protected ATex borderTexture;

        protected float scaleActive = 1.2f;

        protected float scaleInactive = 1f;

        public event Action OnSuccessChangePage;

        public void SetSize(float width, float height)
        {
            Width.Set(width, 0);
            Height.Set(height, 0);
        }

        public void SetHoverImage(ATex texture)
        {
            borderTexture = texture;
        }

        public void SetImage(ATex tex)
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

        public override void LeftClick(UIMouseEvent evt)
        {
            int index = GetPage();
            if (index >= 0)
            {
                GetBookPanel().CurrentDrawingPage = index;
                OnSuccessChangePage?.Invoke();
                Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);
            }
            base.LeftClick(evt);
        }

        public float GetScale()
        {
            return IsMouseHovering ? scaleActive : scaleInactive;
        }
    }
}
