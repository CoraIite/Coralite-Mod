using Coralite.Content.Items.Magike.Altars;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class CraftAltar : KnowledgePage
    {
        public static LocalizedText RemodelPolymerizeAndMagikeCraft { get; private set; }
        public static LocalizedText CraftACraftAltar { get; private set; }
        public static LocalizedText AltarUI { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            RemodelPolymerizeAndMagikeCraft = this.GetLocalization(nameof(RemodelPolymerizeAndMagikeCraft));
            CraftACraftAltar = this.GetLocalization(nameof(CraftACraftAltar));
            AltarUI = this.GetLocalization(nameof(AltarUI));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //标题
            DrawTitleH1(spriteBatch, RemodelPolymerizeAndMagikeCraft, Coralite.MagicCrystalPink);

            Vector2 pos = PageTop + new Vector2(0, TitleHeight);

            //绘制合成坛
            Vector2 picturePos = new Vector2(pos.X, pos.Y + 40);

            Helper.DrawMouseOverScaleTex<BasicAltar>(spriteBatch, picturePos
                , ref _scale1, 4, 5, fadeWithOriginScale: true);

            pos.Y += 120;

            //绘制右边文字
            DrawParaNormal(spriteBatch, CraftACraftAltar, pos.Y, out Vector2 textSize);

            float scale = 0.7f;
            pos.Y += textSize.Y + CoraliteAssets.MagikeChapter1.CraftAltarUI.Height() / 2 * scale + 20;

            //绘制下图
            CoraliteAssets.MagikeChapter1.CraftAltarUI.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale);

            pos.Y += CoraliteAssets.MagikeChapter1.CraftAltarUI.Height() / 2 * scale + 20;

            DrawParaNormal(spriteBatch, AltarUI, pos.Y, out _);
        }
    }
}
