using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PutItemIn : KnowledgePage
    {
        public static LocalizedText WhatIsItemWithMagike { get; private set; }
        public static LocalizedText TurnToMagikeProducer { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(0.9f, 0.1f);
        private ScaleController _scale3 = new ScaleController(1f, 0.2f);
        private ScaleController _scale4 = new ScaleController(1.4f, 0.2f);

        public override void OnInitialize()
        {
            WhatIsItemWithMagike = this.GetLocalization(nameof(WhatIsItemWithMagike));
            TurnToMagikeProducer = this.GetLocalization(nameof(TurnToMagikeProducer));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            _scale3.ResetScale();
            _scale4 = new ScaleController(1.4f, 0.2f);
            _scale4.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);
            //描述段1
            Helper.DrawTextParagraph(spriteBatch, WhatIsItemWithMagike.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            var tex1 = TextureAssets.Item[ModContent.ItemType<MagicCrystal>()].Value;
            var tex11 = TextureAssets.Item[ModContent.ItemType<MagicCrystalBlock>()].Value;
            var tex2 = CoraliteAssets.MagikeChapter1.ItemWithMagike.Value;
            pos.Y += textSize.Y + tex2.Height / 2;

            float width = PageWidth - tex2.Width;
            Vector2 picturePos = new Vector2(Position.X + width / 2, pos.Y);

            #region 绘制左边的物品贴图
            picturePos.X -= tex1.Width * 5 / 2;
            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<MagicCrystal>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            #region 绘制左边的物品贴图2
            picturePos.X = Position.X + width / 2 + tex11.Width * 5 / 2;
            rect = Utils.CenteredRectangle(picturePos, tex11.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale4.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<MagicCrystalBlock>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale4.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex11, picturePos, _scale4, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            picturePos.X = Position.X + PageWidth - tex2.Width / 2;

            #region 绘制右边的图片
            rect = Utils.CenteredRectangle(picturePos, tex2.Size());
            if (rect.MouseScreenInRect())
                _scale2.ToBigSize();
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex2, picturePos, _scale2, 6, Color.DarkGray * 0.5f);
            #endregion

            pos += new Vector2(0, tex2.Height / 2 + 20);

            Helper.DrawTextParagraph(spriteBatch, TurnToMagikeProducer.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            tex2 = CoraliteAssets.MagikeChapter1.TurnToMagikeProducerUI.Value;

            pos.Y += textSize.Y + tex2.Height / 2*1.2f;
            #region 绘制右边的图片
            rect = Utils.CenteredRectangle(pos, tex2.Size() * 1.2f);
            if (rect.MouseScreenInRect())
                _scale3.ToBigSize();
            else
                _scale3.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex2, pos, _scale3, 6, Color.DarkGray * 0.5f);
            #endregion
        }
    }
}
