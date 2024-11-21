using Coralite.Content.Items.Magike.Lens.ExtractLens;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PlaceFirstLens : KnowledgePage
    {
        public static LocalizedText BuildYourFirstMagike { get; private set; }
        public static LocalizedText ExtractLens { get; private set; }
        public static LocalizedText TryMouseHover { get; private set; }
        public static LocalizedText FourWayPlace { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(0.9f, 0.1f);

        public override void OnInitialize()
        {
            BuildYourFirstMagike = this.GetLocalization(nameof(BuildYourFirstMagike));
            ExtractLens = this.GetLocalization(nameof(ExtractLens));
            TryMouseHover = this.GetLocalization(nameof(TryMouseHover));
            FourWayPlace = this.GetLocalization(nameof(FourWayPlace));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            //标题
            Utils.DrawBorderStringBig(spriteBatch, BuildYourFirstMagike.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);
            pos += new Vector2(0, 60);

            Helper.DrawTextParagraph(spriteBatch, ExtractLens.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            var tex1 = TextureAssets.Item[ModContent.ItemType<BasicExtractLens>()].Value;
            var tex2 = CoraliteAssets.MagikeChapter1.PlaceFirstLens.Value;
            pos.Y += textSize.Y + tex2.Height / 2;

            Vector2 picturePos = new Vector2(Position.X + (PageWidth - tex2.Width) / 2, pos.Y);

            #region 绘制左边的透镜贴图
            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicExtractLens>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1 .ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);

            Utils.DrawBorderString(spriteBatch, TryMouseHover.Value, picturePos+new Vector2(0,-tex1.Height*1.5f), Coralite.MagicCrystalPink
                , 1f, 0.5f, 0.5f);

            #endregion

            picturePos.X = Position.X + PageWidth - tex2.Width / 2;

            #region 绘制右边的图片
            rect = Utils.CenteredRectangle(picturePos, tex2.Size());
            if (rect.MouseScreenInRect())
                _scale2.ToBigSize();
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex2, picturePos, _scale2, 10, Color.DarkGray * 0.75f);
            #endregion

            pos += new Vector2(0, tex2.Height/2 + 20);
            Helper.DrawTextParagraph(spriteBatch, FourWayPlace.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);
        }
    }
}
