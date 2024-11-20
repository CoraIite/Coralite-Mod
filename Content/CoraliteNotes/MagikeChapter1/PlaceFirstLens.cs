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

        private float _scale1 ;
        private float _scale2;

        public override void OnInitialize()
        {
            BuildYourFirstMagike = this.GetLocalization(nameof(BuildYourFirstMagike));
            ExtractLens = this.GetLocalization(nameof(ExtractLens));
            TryMouseHover = this.GetLocalization(nameof(TryMouseHover));
            FourWayPlace = this.GetLocalization(nameof(FourWayPlace));
        }

        public override void Recalculate()
        {
            _scale1 = 1.3f;
            _scale2 = 0.8f;
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
                _scale1 = Helper.Lerp(_scale1, 1.5f, 0.1f);
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicExtractLens>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1 = Helper.Lerp(_scale1, 1.4f, 0.1f);

            spriteBatch.Draw(tex1, picturePos + Vector2.One * Helper.Lerp(0, 5, (_scale1 -1.4f) / 0.1f), null,
                new Color(40,40,40) * 0.75f, 0, tex1.Size() / 2, _scale1, 0, 0);
            spriteBatch.Draw(tex1, picturePos, null, Color.White, 0, tex1.Size() / 2, _scale1, 0, 0);

            Utils.DrawBorderString(spriteBatch, TryMouseHover.Value, picturePos+new Vector2(0,-tex1.Height*1.5f), Coralite.MagicCrystalPink
                , 1f, 0.5f, 0.5f);

            #endregion

            picturePos.X = Position.X + PageWidth - tex2.Width / 2;

            #region 绘制右边的图片
            rect = Utils.CenteredRectangle(picturePos, tex2.Size());
            if (rect.MouseScreenInRect())
                _scale2 = Helper.Lerp(_scale2, 1f, 0.15f);
            else
                _scale2 = Helper.Lerp(_scale2, 0.9f, 0.15f);

            spriteBatch.Draw(tex2, picturePos + Vector2.One * Helper.Lerp(0, 10, (_scale2 - 0.9f) / 0.1f), null,
                Color.DarkGray * 0.75f, 0, tex2.Size() / 2, _scale2, 0, 0);
            spriteBatch.Draw(tex2, picturePos, null, Color.White, 0, tex2.Size() / 2, _scale2, 0, 0);
            #endregion

            pos += new Vector2(0, tex2.Height/2 + 20);
            Helper.DrawTextParagraph(spriteBatch, FourWayPlace.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);
        }
    }
}
