using Coralite.Content.Items.Magike.Altars;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class CraftAltar : KnowledgePage
    {
        public static LocalizedText RemodelPolymerizeAndMagikeCraft { get; private set; }
        public static LocalizedText CraftACraftAltar { get; private set; }
        public static LocalizedText PutMainItemIntoAltar { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            RemodelPolymerizeAndMagikeCraft = this.GetLocalization(nameof(RemodelPolymerizeAndMagikeCraft));
            CraftACraftAltar = this.GetLocalization(nameof(CraftACraftAltar));
            PutMainItemIntoAltar = this.GetLocalization(nameof(PutMainItemIntoAltar));
        }

        public override void Recalculate()
        {
            _scale1 = new ScaleController(1.2f, 0.2f);
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);
            //标题
            Utils.DrawBorderStringBig(spriteBatch, RemodelPolymerizeAndMagikeCraft.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 50);

            #region 绘制左边的合成坛

            var tex1 = TextureAssets.Item[ModContent.ItemType<BasicAltar>()].Value;
            Vector2 picturePos = new Vector2(pos.X - 120 - tex1.Width / 2 * 5, pos.Y+tex1.Height);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicAltar>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);

            #endregion

            //绘制右边文字
            float width = PageWidth - tex1.Width * 5-80;
            Helper.DrawTextParagraph(spriteBatch, CraftACraftAltar.Value, width, new Vector2(Position.X + tex1.Width * 5+50, pos.Y), out Vector2 textSize);

            pos.Y += Math.Max(textSize.Y, tex1.Height * 5) + 20;

        }
    }
}
