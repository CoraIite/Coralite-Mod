using Coralite.Content.Items.BossSummons;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class SlimePage2 : KnowledgePage
    {
        public static LocalizedText GelInvitationDescription { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            GelInvitationDescription = this.GetLocalization(nameof(GelInvitationDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);

            #region 绘制史莱姆树苗
            Texture2D tex = TextureAssets.Item[ModContent.ItemType<GelInvitation>()].Value;

            float width = PageWidth - 60 - tex.Width * 3;
            Helper.DrawTextParagraph(spriteBatch, GelInvitationDescription.Value, width, new Vector2(Position.X + 40 + tex.Width * 3, pos.Y), out Vector2 textSize);

            Vector2 picturePos = new Vector2(pos.X - 180 - tex.Width / 2 * 3, pos.Y + textSize.Y / 2);

            Rectangle rect = Utils.CenteredRectangle(picturePos, tex.Size() * 5f);
            if (rect.MouseScreenInRect())
            {
                _scale1.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<GelInvitation>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            float scale1 = 1f;

            tex = CoraliteAssets.Slime1.SlimeEmperor.Value;

            //绘制图2
            pos = Position + new Vector2(PageWidth / 2, PageHeight - 60 - tex.Height * scale1 / 2);
            tex.QuickCenteredDraw(spriteBatch, pos, scale: scale1);
        }
    }
}
