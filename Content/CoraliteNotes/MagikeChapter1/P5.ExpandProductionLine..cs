using Coralite.Content.Items.Magike.Refractors;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class ExpandProductionLine : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Awkward { get; private set; }
        public static LocalizedText Refractor { get; private set; }
        public static LocalizedText UseMoreRefractor { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.2f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Awkward = this.GetLocalization(nameof(Awkward));
            Refractor = this.GetLocalization(nameof(Refractor));
            UseMoreRefractor = this.GetLocalization(nameof(UseMoreRefractor));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            //标题
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 60);

            Texture2D tex = CoraliteAssets.MagikeChapter1.AwkwardTime.Value;
            float scale1 = 0.8f;

            pos.Y += tex.Height * scale1 / 2;

            //绘制图1
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);
            pos.Y += 30 + tex.Height * scale1 / 2;

            Helper.DrawTextParagraph(spriteBatch, Awkward.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            pos.Y += 20 + textSize.Y;
            #region 绘制广角镜

            tex = TextureAssets.Item[ModContent.ItemType<BasicRefractor>()].Value;

            float width = PageWidth - tex.Width * 5 - 40;
            Helper.DrawTextParagraph(spriteBatch, Refractor.Value, width, new Vector2(Position.X + tex.Width * 5 + 20, pos.Y), out textSize);

            Vector2 picturePos = new Vector2(pos.X - 190 - tex.Width / 2 * 5, pos.Y + textSize.Y / 2);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicRefractor>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            pos.Y += textSize.Y + 10;

            Helper.DrawTextParagraph(spriteBatch, UseMoreRefractor.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            tex = CoraliteAssets.MagikeChapter1.Refractors.Value;
            pos.Y += textSize.Y + 10 + tex.Height * scale1 / 2;

            //绘制图2
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);
        }
    }
}
