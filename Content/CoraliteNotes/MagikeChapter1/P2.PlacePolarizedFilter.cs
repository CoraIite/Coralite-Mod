using Coralite.Content.Items.Magike;
using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PlacePolarizedFilter : KnowledgePage
    {
        public static LocalizedText NeedPolarizedFilter { get; private set; }
        public static LocalizedText HowToUsePolarizedFilter { get; private set; }
        public static LocalizedText SeeItemToolTip { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale3 = new ScaleController(0.8f, 0.1f);

        public override void OnInitialize()
        {
            NeedPolarizedFilter = this.GetLocalization(nameof(NeedPolarizedFilter));
            HowToUsePolarizedFilter = this.GetLocalization(nameof(HowToUsePolarizedFilter));
            SeeItemToolTip = this.GetLocalization(nameof(SeeItemToolTip));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            _scale3 = new ScaleController(0.8f, 0.1f);
            _scale3.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);
            //描述段1
            Helper.DrawTextParagraph(spriteBatch, NeedPolarizedFilter.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            pos.Y += textSize.Y-5;

            var tex1 = TextureAssets.Item[ModContent.ItemType<BasicFilter>()].Value;
            var ArrowTex = CoraliteAssets.MagikeChapter1.PlacePolarizedFilterArrow.Value;

            pos.Y += tex1.Height / 2 * 4;
            Vector2 picturePos = new Vector2(pos.X - ArrowTex.Width / 2 - tex1.Width / 2 * 4, pos.Y);

            #region 绘制空白滤镜
            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicFilter>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            #region 绘制偏振滤镜
            tex1 = TextureAssets.Item[ModContent.ItemType<MagicCrystalPolarizedFilter>()].Value;
            picturePos.X = pos.X + ArrowTex.Width / 2 + tex1.Width / 2 * 4;
            rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);

            if (rect.MouseScreenInRect())
            {
                _scale2.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<MagicCrystalPolarizedFilter>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale2, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            //绘制中间箭头
            spriteBatch.Draw(ArrowTex, pos, null, Color.White, 0, ArrowTex.Size() / 2, 1, 0, 0);

            pos.Y += tex1.Height / 2 * 4;
            //描述段2
            Helper.DrawTextParagraph(spriteBatch, HowToUsePolarizedFilter.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            pos.Y += textSize.Y;

            #region 绘制偏振滤镜图片
            tex1 = CoraliteAssets.MagikeChapter1.PlacePolarizedFilter.Value;
            pos.Y += tex1.Height / 2;
            rect = Utils.CenteredRectangle(pos, tex1.Size());

            if (rect.MouseScreenInRect())
                _scale3.ToBigSize();
            else
                _scale3.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, pos, _scale3, 10, Color.DarkGray * 0.75f);
            #endregion

            pos.Y += tex1.Height / 2+10;
            Helper.DrawTextParagraph(spriteBatch, SeeItemToolTip.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);
        }
    }
}
