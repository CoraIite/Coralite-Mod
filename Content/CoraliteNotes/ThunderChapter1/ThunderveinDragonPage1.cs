using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.ThunderChapter1
{
    public class ThunderveinDragonPage1 : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText LightningRodsDescription { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            LightningRodsDescription = this.GetLocalization(nameof(LightningRodsDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.ThunderveinYellow, 1, 0.5f, 0.5f);

            Vector2 pos = Position + new Vector2(PageWidth / 2, 140);

            #region 绘制
            Texture2D tex = TextureAssets.Item[ModContent.ItemType<LightningRods>()].Value;

            float width = PageWidth - 60 - tex.Width * 3;
            Helper.DrawTextParagraph(spriteBatch, LightningRodsDescription.Value, width, new Vector2(Position.X + 60 + tex.Width * 3, pos.Y), out Vector2 textSize);

            Vector2 picturePos = new Vector2(pos.X - 220 - tex.Width / 2 * 3, pos.Y + textSize.Y / 2);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<LightningRods>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            float scale1 = 1f;

            tex = CoraliteAssets.Thunder1.ThunderveidDragon.Value;

            //绘制图2
            pos = Position + new Vector2(PageWidth / 2, PageHeight - 40 - tex.Height * scale1 / 2);
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);
        }
    }
}
