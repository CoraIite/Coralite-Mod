using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairyUITextPanel<T> : UIPanel
    {
        protected T _text;
        protected float _textScale = 1f;
        protected Vector2 _textSize = Vector2.Zero;
        protected bool _isLarge;
        protected Color _color = Color.White;
        protected bool _drawPanel = true;
        public float TextHAlign = 0.5f;
        public bool HideContents;
        private string _asterisks;

        public bool IsLarge => _isLarge;

        public bool DrawPanel
        {
            get
            {
                return _drawPanel;
            }
            set
            {
                _drawPanel = value;
            }
        }

        public float TextScale
        {
            get
            {
                return _textScale;
            }
            set
            {
                _textScale = value;
            }
        }

        public Vector2 TextSize => _textSize;

        public string Text
        {
            get
            {
                if (_text != null)
                    return _text.ToString();

                return "";
            }
        }

        public Color TextColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public FairyUITextPanel(T text, float textScale = 1f, bool large = false)
            : base(FairyEncyclopedia.FairyPanelBackGround
            , FairyEncyclopedia.FairyPanelBorder, 12, 20)
        {
            SetText(text, textScale, large);
        }

        public override void Recalculate()
        {
            SetText(_text, _textScale, _isLarge);
            base.Recalculate();
        }

        public void SetText(T text)
        {
            SetText(text, _textScale, _isLarge);
        }

        public virtual void SetText(T text, float textScale, bool large)
        {
            DynamicSpriteFont dynamicSpriteFont = large ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
            Vector2 textSize = ChatManager.GetStringSize(dynamicSpriteFont, text.ToString(), new Vector2(textScale));
            textSize.Y = (large ? 32f : 16f) * textScale;

            _text = text;
            _textScale = textScale;
            _textSize = textSize;
            _isLarge = large;
            MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
            MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_drawPanel)
                base.DrawSelf(spriteBatch);

            DrawText(spriteBatch);
        }

        protected void DrawText(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();
            if (_isLarge)
                pos.Y -= 10f * _textScale * _textScale;
            else
                pos.Y -= 2f * _textScale;

            pos.X += (innerDimensions.Width - _textSize.X) * TextHAlign;
            string text = Text;
            if (HideContents)
            {
                if (_asterisks == null || _asterisks.Length != text.Length)
                    _asterisks = new string('*', text.Length);

                text = _asterisks;
            }

            if (_isLarge)
                Utils.DrawBorderStringBig(spriteBatch, text, pos, _color, _textScale);
            else
                Utils.DrawBorderString(spriteBatch, text, pos, _color, _textScale);
        }
    }
}