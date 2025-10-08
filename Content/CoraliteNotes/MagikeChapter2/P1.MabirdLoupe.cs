using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    [VaultLoaden(AssetDirectory.NoteMagikeS2)]
    public class MabirdLoupe : KnowledgePage
    {
        public static LocalizedText LoupeDescription { get; private set; }

        public static ATex LoupeTex { get; private set; }

        private ScaleController _scale1 = new ScaleController(1f, 0.2f);

        public override void OnInitialize()
        {
            LoupeDescription = this.GetLocalization(nameof(LoupeDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);

            #region 绘制物品贴图
            var tex1 = TextureAssets.Item[ModContent.ItemType<Items.MagikeSeries2.MabirdLoupe>()].Value;

            Vector2 picturePos = new Vector2(Center.X, pos.Y + 40);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<Items.MagikeSeries2.MabirdLoupe>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);

            #endregion

            pos.Y += 100;

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, LoupeDescription.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            pos.Y += textSize.Y + 40;
            float scale = 1f;
            pos.Y += LoupeTex.Height() / 2 * scale;
            LoupeTex.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale);
        }
    }
}
