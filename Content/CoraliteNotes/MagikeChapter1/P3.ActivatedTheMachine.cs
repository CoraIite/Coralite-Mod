using Coralite.Content.Items.MagikeSeries1;
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
    public class ActivatedTheMachine : KnowledgePage
    {
        public static LocalizedText BuyActivateStaff { get; private set; }
        public static LocalizedText HowToUseActivateStaff { get; private set; }
        public static LocalizedText WorkingBeLike { get; private set; }
        public static LocalizedText HarvestTheProduct { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2= new ScaleController(1f, 0.2f);

        public override void OnInitialize()
        {
            BuyActivateStaff = this.GetLocalization(nameof(BuyActivateStaff));
            HowToUseActivateStaff = this.GetLocalization(nameof(HowToUseActivateStaff));
            WorkingBeLike = this.GetLocalization(nameof(WorkingBeLike));
            HarvestTheProduct = this.GetLocalization(nameof(HarvestTheProduct));
        }

        public override void Recalculate()
        {
            _scale2 = new ScaleController(0.4f, 0.05f);
            _scale1.ResetScale();
            _scale2.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);
            //描述段1
            Helper.DrawTextParagraph(spriteBatch, BuyActivateStaff.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            pos.Y += textSize.Y;

            var tex1 = TextureAssets.Item[ModContent.ItemType<MagikeActivator>()].Value;

            Vector2 picturePos = new Vector2(Position.X, pos.Y + tex1.Height * 4 / 2);

            #region 绘制左边的物品贴图
            picturePos.X += tex1.Width * 4 / 2;
            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<MagikeActivator>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            //在物品贴图右边绘制一段文字
            float width = PageWidth - tex1.Width * 4;
            pos.Y += 40;
            Helper.DrawTextParagraph(spriteBatch, HowToUseActivateStaff.Value, width, new Vector2(Position.X + tex1.Width * 4, pos.Y), out textSize);

            pos.Y += Math.Max(textSize.Y, tex1.Height * 4 - 40) + 10;


            #region 第三段-右图左文字
            var picTex = CoraliteAssets.MagikeChapter1.Working.Value;
            float scale = 0.6f;
            width = PageWidth - picTex.Width* 0.8f;
            picturePos = new Vector2(Position.X + PageWidth - picTex.Width* scale, pos.Y);

            spriteBatch.Draw(picTex, picturePos,null,Color.White,0,Vector2.Zero, scale, 0,0);

            Helper.DrawTextParagraph(spriteBatch, WorkingBeLike.Value, width, new Vector2(Position.X, pos.Y), out textSize);
            #endregion

            //绘制文字与图
            pos.Y += Math.Max(textSize.Y, picTex.Height * scale) + 10;

            Helper.DrawTextParagraph(spriteBatch, HarvestTheProduct.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            pos.Y +=textSize.Y + 10;

            tex1 = CoraliteAssets.MagikeChapter1.HarvestStone.Value;
            pos.Y += tex1.Height *0.4f/ 2;
            rect = Utils.CenteredRectangle(pos, tex1.Size());

            if (rect.MouseScreenInRect())
                _scale2.ToBigSize();
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, pos, _scale2, 10, Color.DarkGray * 0.75f);
        }
    }
}
