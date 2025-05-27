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
            _scale3 = new ScaleController(0.6f, 0.1f);
            _scale3.ResetScale();
            _scale4.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = TitlePos;

            //描述段1
            DrawParaNormal(spriteBatch,WhatIsItemWithMagike,pos.Y, out Vector2 textSize);

            var tex1 = TextureAssets.Item[ModContent.ItemType<MagicCrystal>()].Value;
            var tex11 = TextureAssets.Item[ModContent.ItemType<MagicCrystalBlock>()].Value;
            var tex2 = CoraliteAssets.MagikeChapter1.ItemWithMagike.Value;
            pos.Y += textSize.Y + tex2.Height / 2;

            float width = PageWidth - tex2.Width;
            Vector2 picturePos = new Vector2(Position.X + width / 2, pos.Y);

            // 绘制左边的物品贴图
            picturePos.X -= tex1.Width * 5 / 2;

            Helper.DrawMouseOverScaleTex<MagicCrystal>(spriteBatch, picturePos
                ,ref _scale1, 5,5, fadeWithOriginScale: true);

            // 绘制左边的物品贴图2
            picturePos.X = Position.X + width / 2 + tex11.Width * 5 / 2;
            Helper.DrawMouseOverScaleTex<MagicCrystalBlock>(spriteBatch, picturePos
                , ref _scale4, 5, 5, fadeWithOriginScale: true);

            picturePos.X = Position.X + PageWidth - tex2.Width / 2;

            // 绘制右边的图片
            Helper.DrawMouseOverScaleTex(spriteBatch, tex2, picturePos,ref _scale2, 6);

            pos += new Vector2(0, tex2.Height / 2 + 20);

            //文字段2
            DrawParaNormal(spriteBatch, TurnToMagikeProducer, pos.Y, out textSize);

            Helper.DrawTextParagraph(spriteBatch, TurnToMagikeProducer.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            tex2 = CoraliteAssets.MagikeChapter1.TurnToMagikeProducerUI.Value;

            pos.Y += textSize.Y + tex2.Height / 2 *0.7f+20;

            // 绘制下边的图片
            Helper.DrawMouseOverScaleTex(spriteBatch, tex2, pos,ref _scale3, 6);
        }
    }
}
