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
    public class Prism : KnowledgePage
    {
        public static LocalizedText PrismDescription { get; private set; }
        public static LocalizedText PrismLevels { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.2f, 0.2f);

        public override void OnInitialize()
        {
            PrismDescription = this.GetLocalization(nameof(PrismDescription));
            PrismLevels = this.GetLocalization(nameof(PrismLevels));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);

            #region 绘制棱镜

            Texture2D tex = TextureAssets.Item[ModContent.ItemType<BasicPrism>()].Value;

            float width = PageWidth - tex.Width * 4;
            Helper.DrawTextParagraph(spriteBatch, PrismDescription.Value, width, new Vector2(Position.X + tex.Width * 4, pos.Y), out Vector2 textSize);

            Vector2 picturePos = new Vector2(pos.X - 120 - tex.Width / 2 * 5, pos.Y + textSize.Y / 2);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicPrism>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            float scale1 = 1f;

            tex = CoraliteAssets.MagikeChapter1.PrismLevels.Value;
            pos.Y += textSize.Y + 20 + tex.Height * scale1 / 2;

            //绘制图2
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);

            pos.Y += 20 + tex.Height / 2;

            Helper.DrawTextParagraph(spriteBatch, PrismLevels.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
        }
    }
}
