using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial3/")]
    public class MagikeInterstitial3Page3 : ItemShowPage
    {
        public static LocalizedText SentinelDescription { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.2f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.2f, 0.2f);

        public static ATex CrystallineSentinel { get; private set; }

        public override void OnInitialize()
        {
            SentinelDescription = this.GetLocalization(nameof(SentinelDescription));
            AddImages();
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            base.Recalculate();
        }

        public override void AddImages()
        {
            Vector2 pos =  new Vector2(0, 260);

            ItemShowImage i0 = NewImage<SkarnChest>(pos)
                .SetColor(Coralite.CrystallinePurple);

            ItemShowImage i0_1 = NewImage<Reel_MagikeAdvance>(pos + new Vector2(0, -210))
                .SetColor(Coralite.CrystallinePurple);
            ItemShowImage i0_2 = NewImage<Luminward>(pos + new Vector2(-230, 0), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);
            ItemShowImage i0_3 = NewImage<LuminDye>(pos + new Vector2(-160, -70), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);
            ItemShowImage i0_4 = NewImage<CrystallineHook>(pos + new Vector2(-90, -140), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);

            ItemShowImage i0_5 = NewImage<SkyshipInABottle>(pos + new Vector2(230, 0), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);
            ItemShowImage i0_6 = NewImage<ChalcedonyWing>(pos + new Vector2(160, -70), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);
            ItemShowImage i0_7 = NewImage<UnsentLetter>(pos + new Vector2(90, -140), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);

            i0.AddChainedElement(i0_1);
            i0.AddChainedElement(i0_2);
            i0.AddChainedElement(i0_3);
            i0.AddChainedElement(i0_4);
            i0.AddChainedElement(i0_5);
            i0.AddChainedElement(i0_6);
            i0.AddChainedElement(i0_7);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 70);

            //绘制战斗体和石板
            Helper.DrawMouseOverScaleTex(spriteBatch, CrystallineSentinel.Value, pos + new Vector2(-80, 0), ref _scale1, 5, fadeWithOriginScale: true);
            Helper.DrawMouseOverScaleTex<SkarnKey>(spriteBatch, pos + new Vector2(80, 0), ref _scale2, 5, 5, fadeWithOriginScale: true);

            ItemShowMark.DrawMark(spriteBatch, ItemShowMark.MarkType.Arrow, pos, Coralite.CrystallinePurple);

            pos.Y += 100;

            DrawParaNormal(spriteBatch, SentinelDescription, pos.Y, out _);

            //pos.Y += 60;
            //DrawParaNormal(spriteBatch, BreakBarrierDescription, pos.Y, out size);
            //pos.Y += size.Y + 40;
        }
    }
}
