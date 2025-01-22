using Coralite.Content.Items.Magike.Pedestal;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PolymerizeCraft : KnowledgePage
    {
        public static LocalizedText ALotOfOtherItems { get; private set; }
        public static LocalizedText WhatIsPedestal { get; private set; }
        public static LocalizedText PlacePedestal { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.2f, 0.2f);

        public override void OnInitialize()
        {
            ALotOfOtherItems = this.GetLocalization(nameof(ALotOfOtherItems));
            WhatIsPedestal = this.GetLocalization(nameof(WhatIsPedestal));
            PlacePedestal = this.GetLocalization(nameof(PlacePedestal));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);
            //第一段
            Helper.DrawTextParagraph(spriteBatch, ALotOfOtherItems.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            Texture2D tex1 = CoraliteAssets.MagikeChapter1.PolymerizeCraft.Value;
            float scale1 = 0.8f;
            pos.Y += 10 + textSize.Y + tex1.Height *scale1/ 2;

            tex1.QuickCenteredDraw(spriteBatch, pos,scale: scale1);

            pos.Y += 20 + tex1.Height* scale1 / 2;

            #region 绘制左边的置物台

            tex1 = TextureAssets.Item[ModContent.ItemType<BasicPedestal>()].Value;
            Vector2 picturePos = new Vector2(pos.X - 190 - tex1.Width / 2 * 5, pos.Y + tex1.Height);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicPedestal>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);

            #endregion

            //绘制右边文字
            float width = PageWidth - tex1.Width * 5 - 40;
            Helper.DrawTextParagraph(spriteBatch, WhatIsPedestal.Value, width, new Vector2(Position.X + tex1.Width * 5 + 20, pos.Y), out textSize);

            pos.Y += Math.Max(textSize.Y, tex1.Height * 2.5f) + 10;

            Helper.DrawTextParagraph(spriteBatch, PlacePedestal.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            tex1 = CoraliteAssets.MagikeChapter1.PlacePedestal.Value;
            pos.Y += textSize.Y + 10 + tex1.Height *0.9f/ 2;

            //绘制下图
            tex1.QuickCenteredDraw(spriteBatch, pos,scale:0.9f);
        }
    }
}
