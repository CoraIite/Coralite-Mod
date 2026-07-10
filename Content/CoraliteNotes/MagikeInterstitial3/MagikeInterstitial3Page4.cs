using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    public class MagikeInterstitial3Page4 : ItemShowPage
    {
        public static LocalizedText SentinelLoot { get; private set; }

        public override void OnInitialize()
        {
            SentinelLoot = this.GetLocalization(nameof(SentinelLoot));
            AddImages();
        }

        public override void AddImages()
        {
            Vector2 pos = new Vector2(0, 260);

            ItemShowImage i0 = NewImage<CrystallineEngram>(pos, conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);

            NewImage<BrillantRubiksCube>(pos + new Vector2(0, -320))
                .SetColor(Coralite.CrystallinePurple);

            ItemShowImage i0_1 = NewImage<CrystallineShield>(pos + new Vector2(0, -210))
                .SetColor(Coralite.CrystallinePurple);
            ItemShowImage i0_2 = NewImage<CrystallineTriggerPrecise>(pos + new Vector2(-230, 0), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);
            ItemShowImage i0_3 = NewImage<CrystallineTriggerScatter>(pos + new Vector2(-125, -105), conditions: CoraliteConditions.DownedCrystallineSentinel)
                .SetColor(Coralite.CrystallinePurple);

            i0.AddChainedElement(i0_1);
            i0.AddChainedElement(i0_2);
            i0.AddChainedElement(i0_3);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, SentinelLoot, Position.Y + 40, out _);
        }
    }
}
