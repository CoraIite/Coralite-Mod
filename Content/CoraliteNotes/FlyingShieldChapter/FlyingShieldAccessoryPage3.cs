using Coralite.Content.Items.FlyingShields.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldAccessoryPage3 : ItemShowPage
    {
        public static LocalizedText DashSeries { get; private set; }

        public override void OnInitialize()
        {
            DashSeries = this.GetLocalization(nameof(DashSeries));
            AddImages();
        }

        public override void AddImages()
        {
            int x1 = -270;
            int y1 = -160;

            ItemShowImage i0_0 = NewImage(ItemID.DemoniteBar, new Vector2(x1, y1 + 40), Readfragment.KnowledgeButtonType.None)
                .SetColor(Coralite.CorruptionPurple);
            ItemShowImage i0_1 = NewImage(ItemID.CrimtaneBar, new Vector2(x1, y1 - 40), Readfragment.KnowledgeButtonType.None)
                .SetColor(Coralite.CrimsonRed);
            x1 += 70;

            ItemShowImage i1 = NewImage<ShieldSpring>(new Vector2(x1, y1))
                .SetColor(Colors.RarityBlue);
            i0_0.AddChainedElement(i1);
            i0_1.AddChainedElement(i1);

            x1 += 80 + 60;
            ItemShowImage i2 = NewImage<AlloySpring>(new Vector2(x1+30, y1-20), conditions: Condition.Hardmode)
                .SetColor(Colors.RarityPink);
            ItemShowImage i3 = NewImage<HeavyWedges>(new Vector2(x1-55, y1-50))
                .SetColor(Colors.RarityBlue);
            ItemShowImage i4 = NewImage<FlyingShieldBattleGuide>(new Vector2(x1-75, y1+50))
                .SetColor(Colors.RarityGreen);
            ItemShowImage i5 = NewImage<PossessedChest>(new Vector2(x1+5, y1 + 60), conditions: Condition.Hardmode)
                .SetColor(Colors.RarityPink);

            i1.AddChainedElement(i2);
            i3.AddChainedElement(i2);
            i4.AddChainedElement(i2);
            i5.AddChainedElement(i2);

            x1 += 80 + 60;

            ItemShowImage i6 = NewImage<GravitationalCatapult>(new Vector2(x1, y1 + 5), conditions: Condition.DownedSolarPillar)
                .SetColor(Colors.RarityDarkRed);

            i2.AddChainedElement(i6);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, DashSeries, Position.Y + 40, out _);
        }
    }
}
