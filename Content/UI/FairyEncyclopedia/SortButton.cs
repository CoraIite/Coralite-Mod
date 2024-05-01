using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class SortButton:UIElement
    {
        private readonly Asset<Texture2D> ButtonTex;
        public readonly FairyEncyclopedia.SortStyle sortStyle;

        private readonly LocalizedText description; 

        private float _visibilityActive = 1f;

        private float _visibilityInactive = 0.4f;

        public SortButton(Asset<Texture2D> ButtonTex, FairyEncyclopedia.SortStyle sortStyle,LocalizedText description)
        {
            this.ButtonTex = ButtonTex;
            this.sortStyle = sortStyle;
            this.description = description;
            Width.Set(ButtonTex.Width(), 0f);
            Height.Set(ButtonTex.Height(), 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            spriteBatch.Draw(ButtonTex.Value, dimensions.Position(), Color.White * (IsMouseHovering ? _visibilityActive : _visibilityInactive));
            if (IsMouseHovering)
            {
                Main.instance.MouseText(description == null ? "" : description.Value);
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/ButtonTick", 0.2f, 0);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }

        public void SetVisibility(float whenActive, float whenInactive)
        {
            _visibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
            _visibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
        }
    }
}
