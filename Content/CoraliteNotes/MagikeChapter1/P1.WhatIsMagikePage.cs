using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class WhatIsMagikePage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        private ScaleController _scale =new ScaleController(0.9f,0.1f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        public override void Recalculate()
        {
            _scale.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            //标题
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 60);

            //描述段
            Helper.DrawTextParagraph(spriteBatch, Description.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            var tex = CoraliteAssets.MagikeChapter1.WhatIsMagike.Value;
            pos.Y += textSize.Y + 20 + tex.Height / 2;

            //绘制图片
            Rectangle rect = Utils.CenteredRectangle(pos, tex.Size());
            if (rect.MouseScreenInRect())
                _scale.ToBigSize();
            else
                _scale.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, pos, _scale, 20, Color.DarkGray * 0.75f);
        }
    }
}
