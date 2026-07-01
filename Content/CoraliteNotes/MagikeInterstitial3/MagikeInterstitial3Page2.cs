using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial3/")]
    public class MagikeInterstitial3Page2 : KnowledgePage
    {
        public static LocalizedText CrystallineMagikeDescription { get; private set; }
        public static LocalizedText BreakBarrierDescription { get; private set; }
        public static LocalizedText ExBarrierDescription { get; private set; }

        public static ATex CrystallineBarrier { get; private set; }
        public static ATex CrystallineBarrierTemporary { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.5f, 0.2f);
        private ScaleController _scale3 = new ScaleController(1.5f, 0.2f);
        private ScaleController _scale4 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            CrystallineMagikeDescription = this.GetLocalization(nameof(CrystallineMagikeDescription));
            BreakBarrierDescription = this.GetLocalization(nameof(BreakBarrierDescription));
            ExBarrierDescription = this.GetLocalization(nameof(ExBarrierDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            _scale3.ResetScale();
            _scale4.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0,30);
            DrawParaNormal(spriteBatch, CrystallineMagikeDescription, pos.Y, out Vector2 size);
            pos.Y += size.Y + 40;

            //绘制蕴魔水晶和矽卡岩
            Helper.DrawMouseOverScaleTex<CrystallineMagike>(spriteBatch, pos + new Vector2(-80, 0), ref _scale1, 5, 5, fadeWithOriginScale: true);
            Helper.DrawMouseOverScaleTex<Skarn>(spriteBatch, pos + new Vector2(80, 0), ref _scale2, 5, 5, fadeWithOriginScale: true);

            pos.Y += 60;
            DrawParaNormal(spriteBatch, BreakBarrierDescription, pos.Y, out  size);
            pos.Y += size.Y + 40;

            //绘制两个屏障

            Helper.DrawMouseOverScaleTex(spriteBatch, CrystallineBarrier.Value, pos + new Vector2(-80, 0), ref _scale3, 10, fadeWithOriginScale:true);
            Helper.DrawMouseOverScaleTex(spriteBatch, CrystallineBarrierTemporary.Value, pos + new Vector2(80, 0), ref _scale4, 10, fadeWithOriginScale:true);

            //绘制箭头
            ItemShowMark.DrawMark(spriteBatch, ItemShowMark.MarkType.Arrow, pos, Coralite.CrystallinePurple);

            pos.Y += 60;
            DrawParaNormal(spriteBatch, ExBarrierDescription, pos.Y, out _);
        }
    }
}
