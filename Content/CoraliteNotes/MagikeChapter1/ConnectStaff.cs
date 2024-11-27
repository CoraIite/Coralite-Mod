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
    public class ConnectStaff:KnowledgePage
    {
        public static LocalizedText BuyConnectStaff { get; private set; }
        public static LocalizedText HowToMoveMagike { get; private set; }
        public static LocalizedText ConnectStep1 { get; private set; }
        public static LocalizedText ConnectStep2 { get; private set; }
        public static LocalizedText UseConnectStaff { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.4f, 0.2f);

        public override void OnInitialize()
        {
            BuyConnectStaff = this.GetLocalization(nameof(BuyConnectStaff));
            HowToMoveMagike = this.GetLocalization(nameof(HowToMoveMagike));
            ConnectStep1 = this.GetLocalization(nameof(ConnectStep1));
            ConnectStep2 = this.GetLocalization(nameof(ConnectStep2));
            UseConnectStaff = this.GetLocalization(nameof(UseConnectStaff));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);
            //描述段1
            Helper.DrawTextParagraph(spriteBatch, BuyConnectStaff.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            pos.Y += textSize.Y;

            var tex1 = CoraliteAssets.MagikeChapter1.CrystalRobot.Value;
            var ArrowTex = CoraliteAssets.MagikeChapter1.PlacePolarizedFilterArrow.Value;

            pos.Y += tex1.Height / 2 * 4;
            Vector2 picturePos = new Vector2(pos.X - ArrowTex.Width / 2 - tex1.Width / 2 * 4, pos.Y);

            #region 绘制水晶生命体
            Rectangle rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);
            if (rect.MouseScreenInRect())
                _scale1.ToBigSize();
            else
                _scale1.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale1, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion

            #region 绘制连接杖
            tex1 = TextureAssets.Item[ModContent.ItemType<MagConnectStaff>()].Value;
            picturePos.X = pos.X + ArrowTex.Width / 2 + tex1.Width / 2 * 4;
            rect = Utils.CenteredRectangle(picturePos, tex1.Size() * 4f);

            if (rect.MouseScreenInRect())
            {
                _scale2.ToBigSize();
                Main.HoverItem = ContentSamples.ItemsByType[ModContent.ItemType<MagConnectStaff>()].Clone();
                Main.hoverItemName = "a";
            }
            else
                _scale2.ToNormalSize();

            Helper.DrawMouseOverScaleTex(spriteBatch, tex1, picturePos, _scale2, 5, new Color(40, 40, 40) * 0.5f, true);
            #endregion


            //绘制中间箭头
            spriteBatch.Draw(ArrowTex, pos, null, Color.White, 0, ArrowTex.Size() / 2, 1, 0, 0);

            pos.Y += tex1.Height / 2 * 4;

            Helper.DrawTextParagraph(spriteBatch, HowToMoveMagike.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);

            pos.Y += textSize.Y + 40;
            pos.X -= 20;

            //绘制两段操作描述
            Helper.DrawText(spriteBatch, ConnectStep1.Value, PageWidth, pos, Vector2.One / 2, Vector2.One, new Color(50, 50, 50), Color.White, out textSize);
            tex1 = CoraliteAssets.MagikeChapter1.ConnectStaff1.Value;
            pos.Y += textSize.Y - 8 + tex1.Height / 2;
            spriteBatch.Draw(tex1, pos, null, Color.White, 0, tex1.Size() / 2, 1, 0, 0);

            pos.Y += tex1.Height / 2 + 40;

            Helper.DrawText(spriteBatch, ConnectStep2.Value, PageWidth, pos, Vector2.One / 2, Vector2.One, new Color(50, 50, 50), Color.White, out textSize);
            tex1 = CoraliteAssets.MagikeChapter1.ConnectStaff2.Value;
            pos.Y += textSize.Y+ tex1.Height / 2;
            spriteBatch.Draw(tex1, pos, null, Color.White, 0, tex1.Size() / 2, 1, 0, 0);

            pos.Y += tex1.Height / 2 + 30;

            Helper.DrawTextParagraph(spriteBatch, UseConnectStaff.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);
        }
    }
}
