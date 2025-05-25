using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
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
            DrawTitle(spriteBatch, Title, Coralite.ThunderveinYellow);

            //绘制图
            CoraliteAssets.Thunder1
                .ThunderveidDragon.Value.QuickBottomDraw(spriteBatch, Bottom+new Vector2(0,-10));

            DrawParaNormal(spriteBatch, LightningRodsDescription,Position.Y+ TitleHeight, out _);

            #region 绘制避雷针

            Vector2 picturePos = new Vector2(Center.X +75, Center.Y + 305);
            Helper.DrawMouseOverScaleTex<LightningRods>(spriteBatch, picturePos
                , ref _scale1, 4, 5, fadeWithOriginScale: true);

            #endregion
        }
    }
}
