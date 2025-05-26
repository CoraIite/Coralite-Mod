using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PlacePolarizedFilter2 : KnowledgePage
    {
        public static LocalizedText HowToUsePolarizedFilter { get; private set; }
        public static LocalizedText SeeItemToolTip { get; private set; }

        private ScaleController _scale = new ScaleController(0.9f, 0.05f);

        public override void OnInitialize()
        {
            HowToUsePolarizedFilter = this.GetLocalization(nameof(HowToUsePolarizedFilter));
            SeeItemToolTip = this.GetLocalization(nameof(SeeItemToolTip));
        }

        public override void Recalculate()
        {
            _scale.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = TitlePos;

            //描述段1
            DrawParaNormal(spriteBatch, HowToUsePolarizedFilter, pos.Y, out Vector2 textSize);
            pos.Y += textSize.Y;

            //绘制偏振滤镜图片

            Texture2D tex1 = CoraliteAssets.MagikeChapter1.PlacePolarizedFilter.Value;
            pos.Y += tex1.Height / 2;

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, pos, ref _scale, 4);

            pos.Y += tex1.Height / 2 + 20;

            //绘制描述段2
            Helper.DrawTextParagraph(spriteBatch, SeeItemToolTip.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);
        }
    }
}
