using Coralite.Content.Items.BossSummons;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
            tex.QuickBottomDraw(spriteBatch, pos, scale: scale1);

            DrawTitle(spriteBatch, Title, Coralite.RedJadeRed);

            pos = Position + new Vector2(PageWidth / 2, TitleHeight);
            DrawParaNormal(spriteBatch, RedBerryDescription, pos.Y, out _);

            #region 绘制赤玉
            Vector2 picturePos = Position + new Vector2(475, PageHeight - 180);

            Helper.DrawMouseOverScaleTex<RedBerry>(spriteBatch, picturePos
                , ref _scale1, 4, 5, fadeWithOriginScale: true);
            #endregion
        }
    }
}
