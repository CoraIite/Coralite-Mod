using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.UI
{
    /// <summary>
    /// 一个用于显示图标并且鼠标悬浮时有文字提示的组件
    /// </summary>
    public class UIInformationIcon : UIElement
    {
        private ATex _tex;
        private LocalizedText _text;

        public UIInformationIcon(ATex tex)
        {
            _tex = tex;
            this.SetSize(tex.Width(), tex.Height());
        }

        public void SetText(LocalizedText text)
        {
            _text = text;
        }

        public void SetSize()
        {
            this.SetSize(_tex.Width(), _tex.Height());
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            _tex.Value.QuickCenteredDraw(spriteBatch, GetDimensions().Center(), Color.White, scale: IsMouseHovering ? 1.05f : 1f);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (_text != null)
                    UICommon.TooltipMouseText(_text.Value);
            }
        }
    }
}
