using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    public class TitleElement : UIElement
    {
        private readonly ATex _tex;
        private readonly LocalizedText _text;
        private readonly Vector2 _textOffset;
        private readonly Color _textColor;

        public TitleElement(ATex tex, LocalizedText text, float height, Vector2 textOffset, Color textColor)
        {
            _tex = tex;
            _text = text;
            _textOffset = textOffset;
            _textColor = textColor;

            this.SetSize(0, height, 1, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 pos = calculatedStyle.Position();

            Texture2D tex = _tex.Value;

            spriteBatch.Draw(tex, pos + new Vector2(calculatedStyle.Width / 2, calculatedStyle.Height-5), null, Color.White, 0, new Vector2(tex.Width / 2, tex.Height), 1.1f, 0, 0);

            Utils.DrawBorderStringBig(spriteBatch, _text.Value, pos + _textOffset + new Vector2(calculatedStyle.Width / 2, calculatedStyle.Height / 2)
                , _textColor, 0.65f, 0.5f, 0.5f);
        }
    }
}
