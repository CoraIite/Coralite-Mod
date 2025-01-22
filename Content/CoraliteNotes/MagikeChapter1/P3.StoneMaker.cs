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
    public class StoneMaker : KnowledgePage
    {
        public static LocalizedText UseMagikeToDoSomething { get; private set; }
        public static LocalizedText MakeAStoneMaker { get; private set; }
        public static LocalizedText InsertPolarizedFilter { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(0.7f, 0.1f);

        public override void OnInitialize()
        {
            UseMagikeToDoSomething = this.GetLocalization(nameof(UseMagikeToDoSomething));
            MakeAStoneMaker = this.GetLocalization(nameof(MakeAStoneMaker));
            InsertPolarizedFilter = this.GetLocalization(nameof(InsertPolarizedFilter));
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
            Utils.DrawBorderStringBig(spriteBatch, UseMagikeToDoSomething.Value, pos, Coralite.MagicCrystalPink
                , 0.8f, 0.5f, 0.5f);

            pos += new Vector2(0, 50);

            var tex1 = TextureAssets.Item[ModContent.ItemType<Items.Magike.Factorys.StoneMaker>()].Value;

            Vector2 picturePos = new Vector2(Position.X, pos.Y + tex1.Height * 5 / 2);

            #region 绘制左边的物品贴图
            picturePos.X += tex1.Width * 5 / 2;
            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<Items.Magike.Factorys.StoneMaker>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            //在物品贴图右边绘制一段文字
            float width = PageWidth - tex1.Width * 5;
            Helper.DrawTextParagraph(spriteBatch, MakeAStoneMaker.Value, width, new Vector2(Position.X + tex1.Width * 5, pos.Y), out Vector2 textSize);

            pos.Y += Math.Max(textSize.Y, tex1.Height * 5) + 20;

            //描述段2
            Helper.DrawTextParagraph(spriteBatch, InsertPolarizedFilter.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);

            var tex = CoraliteAssets.MagikeChapter1.StoneMakerExample.Value;
            pos.Y += textSize.Y + tex.Height * 0.8f / 2;

            #region 绘制图片
            rect = Utils.CenteredRectangle(pos, tex.Size() * 0.7f);
            if (rect.MouseScreenInRect())
                _scale2.ToBigSize();
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, pos, _scale2, 8, Color.DarkGray * 0.75f);
            #endregion
        }
    }
}
