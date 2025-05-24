using Coralite.Content.Items.BossSummons;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    public class RedJadePage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText RedBerryDescription { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            RedBerryDescription = this.GetLocalization(nameof(RedBerryDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float scale1 = 1f;

            Texture2D tex = CoraliteAssets.RedJade.Rediancie.Value;

            //绘制图2
            Vector2 pos = Bottom;
            spriteBatch.Draw(tex, pos, null, Color.White, 0, new Vector2(tex.Width / 2, tex.Height), scale1, 0, 0);

            Utils.DrawBorderStringBig(spriteBatch, Title.Value, PageTop+new Vector2(0,20), Coralite.RedJadeRed, 1, 0.5f, 0f);

             pos = Position + new Vector2(PageWidth / 2, CoraliteNotePanel.TitleHeight);
            Helper.DrawTextParagraph(spriteBatch, RedBerryDescription.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            #region 绘制赤玉
            tex = TextureAssets.Item[ModContent.ItemType<RedBerry>()].Value;

            Vector2 picturePos =Position+ new Vector2(475, PageHeight-180);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<RedBerry>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion
        }
    }
}
