using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Placeable;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class SlimePage1 : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText SlimeTreeDescription { get; private set; }
        public static LocalizedText GelFiberDescription { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            SlimeTreeDescription = this.GetLocalization(nameof(SlimeTreeDescription));
            GelFiberDescription = this.GetLocalization(nameof(GelFiberDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, Center + new Vector2(0, -PageWidth / 2), Color.SkyBlue, 1, 0.5f, 0.5f);

            Vector2 pos = Position + new Vector2(PageWidth / 2, 140);

            #region 绘制史莱姆树苗
            Texture2D tex = TextureAssets.Item[ModContent.ItemType<SlimeSapling>()].Value;

            float width = PageWidth - 60 - tex.Width * 3;
            Helper.DrawTextParagraph(spriteBatch, SlimeTreeDescription.Value, width, new Vector2(Position.X + 40 + tex.Width * 3, pos.Y), out Vector2 textSize);

            Vector2 picturePos = new Vector2(pos.X - 180 - tex.Width / 2 * 3, pos.Y + textSize.Y / 2);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<SlimeSapling>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            pos.Y += Math.Max(tex.Height * 2, textSize.Y) + 20;

            #region 绘制凝胶纤维
            tex = TextureAssets.Item[ModContent.ItemType<GelFiber>()].Value;

            width = PageWidth - 60 - tex.Width * 3;
            Helper.DrawTextParagraph(spriteBatch, GelFiberDescription.Value, width, new Vector2(Position.X + 40 + tex.Width * 3, pos.Y), out textSize);

            picturePos = new Vector2(pos.X - 180 - tex.Width / 2 * 3, pos.Y + textSize.Y / 2);

            rect = Utils.CenteredRectangle(picturePos, tex.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale2.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<GelFiber>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale2, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            float scale1 = 0.65f;

            pos.Y += Math.Max(tex.Height * 2, textSize.Y) + 20;
            tex = CoraliteAssets.Slime1.SlimeTree.Value;
            pos.Y += tex.Height * scale1 / 2;

            //绘制史莱姆树
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);
        }
    }
}
