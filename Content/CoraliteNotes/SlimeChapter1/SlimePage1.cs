using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Placeable;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    [AutoLoadTexture(Path =AssetDirectory.NoteSlime1)]
    public class SlimePage1 : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText SlimeTreeDescription { get; private set; }
        public static LocalizedText GelFiberDescription { get; private set; }

        public static ATex SlimeTree {  get; private set; }

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
            DrawTitleH1(spriteBatch, Title, Color.SkyBlue);
            float posY = Position.Y + TitleHeight;

            //绘制史莱姆树
            SlimeTree.Value.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10));

            DrawParaNormal(spriteBatch, SlimeTreeDescription, posY, out Vector2 textSize);

            posY += textSize.Y+20;

            DrawParaNormal(spriteBatch, GelFiberDescription, posY, out _);

            #region 绘制史莱姆树苗

            Vector2 picturePos = new Vector2(Center.X + 195, Center.Y + 90);
            Helper.DrawMouseOverScaleTex<SlimeSapling>(spriteBatch, picturePos
                , ref _scale1, 5, 5, fadeWithOriginScale: true);

            #endregion

            #region 绘制凝胶纤维

            picturePos = new Vector2(Center.X - 95, Center.Y + 125);
            Helper.DrawMouseOverScaleTex<GelFiber>(spriteBatch, picturePos
                , ref _scale2, 5, 5, fadeWithOriginScale: true);
            #endregion
        }
    }
}
