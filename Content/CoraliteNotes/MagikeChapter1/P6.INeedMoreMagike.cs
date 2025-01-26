using Coralite.Content.Items.Magike.Lens.BiomeLens;
using Coralite.Content.Items.Magike.Lens.DayTimeLens;
using Coralite.Content.Items.Magike.Lens.LiquidLens;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class INeedMoreMagike : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText NeedMoreMagike { get; private set; }
        public static LocalizedText BiomeLens { get; private set; }
        public static LocalizedText DayTimeLens { get; private set; }
        public static LocalizedText LiquidLens { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.2f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.2f, 0.2f);
        private ScaleController _scale3 = new ScaleController(1.2f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            NeedMoreMagike = this.GetLocalization(nameof(NeedMoreMagike));
            BiomeLens = this.GetLocalization(nameof(BiomeLens));
            DayTimeLens = this.GetLocalization(nameof(DayTimeLens));
            LiquidLens = this.GetLocalization(nameof(LiquidLens));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            _scale3.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            //标题
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 60);

            Helper.DrawTextParagraph(spriteBatch, NeedMoreMagike.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            pos.Y += 20 + textSize.Y;
            #region 绘制环境透镜

            Texture2D tex = TextureAssets.Item[ModContent.ItemType<ForestLens>()].Value;

            float width = PageWidth - tex.Width * 3 - 20;
            Helper.DrawTextParagraph(spriteBatch, BiomeLens.Value, width, new Vector2(Position.X + tex.Width * 3 + 20, pos.Y), out textSize);

            Vector2 picturePos = new Vector2(pos.X - 175 - tex.Width / 2 * 3, pos.Y + textSize.Y / 2);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<ForestLens>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            pos.Y += textSize.Y + 20;

            #region 绘制光照透镜

            tex = TextureAssets.Item[ModContent.ItemType<SunlightLens>()].Value;

            width = PageWidth - tex.Width * 3 - 20;
            Helper.DrawTextParagraph(spriteBatch, DayTimeLens.Value, width, new Vector2(Position.X + tex.Width * 3 + 20, pos.Y), out textSize);

            picturePos = new Vector2(pos.X - 175 - tex.Width / 2 * 3, pos.Y + textSize.Y / 2);

            rect = Utils.CenteredRectangle(picturePos, tex.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale2.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<SunlightLens>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale2, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            pos.Y += textSize.Y + 20;

            #region 绘制液体透镜

            tex = TextureAssets.Item[ModContent.ItemType<WaterflowLens>()].Value;

            width = PageWidth - tex.Width * 3 - 20;
            Helper.DrawTextParagraph(spriteBatch, LiquidLens.Value, width, new Vector2(Position.X + tex.Width * 3 + 20, pos.Y), out textSize);

            picturePos = new Vector2(pos.X - 175 - tex.Width / 2 * 3, pos.Y + textSize.Y / 2);

            rect = Utils.CenteredRectangle(picturePos, tex.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale3.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<WaterflowLens>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale3.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale3, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion
        }
    }
}
