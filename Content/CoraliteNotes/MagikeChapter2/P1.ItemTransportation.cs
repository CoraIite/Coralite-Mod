using Coralite.Content.Items.Magike.ItemTransmit;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class ItemTransportation : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText MabirdDescription { get; private set; }

        private ScaleController _scale1 = new ScaleController(1f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            MabirdDescription = this.GetLocalization(nameof(MabirdDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.MagicCrystalPink, 1, 0.5f, 0.5f);

            Vector2 pos = Position + new Vector2(0, 140);

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, MabirdDescription.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            pos.Y += textSize.Y;

            var tex1 = TextureAssets.Item[ModContent.ItemType<BasicMabirdNest>()].Value;

            Vector2 picturePos = new Vector2(Center.X, pos.Y + 100);

            #region 绘制物品贴图
            picturePos.X -= 110;
            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<BasicMabirdNest>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);

            tex1 = TextureAssets.Item[ModContent.ItemType<CrystallineMabird>()].Value;

            picturePos.X = Center.X + 110;
            rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale2.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<CrystallineMabird>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale2, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion
        }
    }
}
