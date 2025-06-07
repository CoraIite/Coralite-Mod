using Coralite.Content.Items.Magike;
using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    [AutoLoadTexture(Path = AssetDirectory.NoteMagikeS1)]
    public class PlacePolarizedFilter : KnowledgePage
    {
        public static LocalizedText NeedPolarizedFilter { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.4f, 0.2f);

        public static ATex PolarizedFilterStructure { get; private set; }

        public override void OnInitialize()
        {
            NeedPolarizedFilter = this.GetLocalization(nameof(NeedPolarizedFilter));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //绘制下面的图
            PolarizedFilterStructure.Value.QuickBottomDraw(spriteBatch, Bottom + new Vector2(0, -20));

            var ArrowTex = CoraliteAssets.MagikeChapter1.PlacePolarizedFilterArrow.Value;

            Vector2 pos = TitlePos + new Vector2(0, ArrowTex.Height / 2 + 30);

            Vector2 pictureOffset = new Vector2(ArrowTex.Width / 2 + 60, 0);
            // 绘制空白滤镜
            Helper.DrawMouseOverScaleTex<BasicFilter>(spriteBatch, pos - pictureOffset
                , ref _scale1, 3, 5, fadeWithOriginScale: true);

            // 绘制偏振滤镜
            Helper.DrawMouseOverScaleTex<MagicCrystalPolarizedFilter>(spriteBatch, pos + pictureOffset
                , ref _scale2, 3, 5, fadeWithOriginScale: true);

            //绘制中间箭头
            spriteBatch.Draw(ArrowTex, pos, null, Color.White, 0, ArrowTex.Size() / 2, 1, 0, 0);

            pos.Y += ArrowTex.Height / 2 + 30 + 30;

            //描述段1
            DrawParaNormal(spriteBatch, NeedPolarizedFilter, pos.Y, out Vector2 textSize);
        }
    }
}
