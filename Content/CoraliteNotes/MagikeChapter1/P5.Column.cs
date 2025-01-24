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
    public class Column : KnowledgePage
    {
        public static LocalizedText ColumnDescription { get; private set; }
        public static LocalizedText ColumnStack { get; private set; }
        public static LocalizedText BigColumn { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.2f, 0.2f);

        public override void OnInitialize()
        {
            ColumnDescription = this.GetLocalization(nameof(ColumnDescription));
            ColumnStack = this.GetLocalization(nameof(ColumnStack));
            BigColumn = this.GetLocalization(nameof(BigColumn));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);

            #region 绘制水晶柱

            Texture2D tex = TextureAssets.Item[ModContent.ItemType<BasicColumn>()].Value;

            float width = PageWidth - tex.Width * 4;
            Helper.DrawTextParagraph(spriteBatch, ColumnDescription.Value, width, new Vector2(Position.X + tex.Width * 4 , pos.Y), out Vector2 textSize);

            Vector2 picturePos = new Vector2(pos.X - 120 - tex.Width / 2 * 5, pos.Y + textSize.Y / 2);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicColumn>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            pos.Y += textSize.Y + 10;
            float scale1 = 1f;

            #region 绘制图1

            tex = CoraliteAssets.MagikeChapter1.Columns.Value;

            picturePos = new Vector2(pos.X + tex.Width / 2 * 4, pos.Y + tex.Height / 2);

            tex.QuickCenteredDraw(spriteBatch, picturePos, scale: scale1);

            width = PageWidth - tex.Width * 2.5f;
            Helper.DrawTextParagraph(spriteBatch, ColumnStack.Value, width, new Vector2(Position.X+20, pos.Y + tex.Height * scale1 / 2-50), out textSize);

            #endregion

            pos.Y += tex.Height + 20;

            Helper.DrawTextParagraph(spriteBatch, BigColumn.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            tex = CoraliteAssets.MagikeChapter1.BigColumn.Value;
            pos.Y += textSize.Y + 20 + tex.Height * scale1 / 2;

            //绘制图2
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);
        }
    }
}
