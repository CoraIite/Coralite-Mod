using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.Materials;
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
            int y1 = -140;

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
            ItemShowImage i2 = NewImage<AlloySpring>(new Vector2(x1 + 30, y1 - 20), conditions: Condition.Hardmode)
                .SetColor(Colors.RarityPink);
            ItemShowImage i3 = NewImage<HeavyWedges>(new Vector2(x1 - 55, y1 - 50))
                .SetColor(Colors.RarityBlue);
            ItemShowImage i4 = NewImage<FlyingShieldBattleGuide>(new Vector2(x1 - 75, y1 + 50))
                .SetColor(Colors.RarityGreen);
            ItemShowImage i5 = NewImage<PossessedChest>(new Vector2(x1 + 5, y1 + 60), conditions: Condition.Hardmode)
                .SetColor(Colors.RarityPink);

            i1.AddChainedElement(i2);
            i3.AddChainedElement(i2);
            i4.AddChainedElement(i2);
            i5.AddChainedElement(i2);

            x1 += 80 + 60;

            ItemShowImage i6 = NewImage<GravitationalCatapult>(new Vector2(x1, y1 + 5), conditions: Condition.DownedSolarPillar)
                .SetColor(Colors.RarityDarkRed);

            i2.AddChainedElement(i6);

            x1 = -270;
            y1 = 60;

            ItemShowImage i1_0 = NewImage(ItemID.Torch, new Vector2(x1, y1), Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.Orange);

            x1 += 70;

            ItemShowImage i1_1 = NewImage<FlamingLamps>(new Vector2(x1, y1))
                .SetColor(Colors.RarityBlue);
            i1_0.AddChainedElement(i1_1);

            x1 += 80 + 30;

            ItemShowImage i1_2 = NewImage<LavaLamp>(new Vector2(x1, y1 - 15), conditions: Condition.DownedEowOrBoc)
                .SetColor(Colors.RarityOrange);
            i1_1.AddChainedElement(i1_2);

            x1 += 80 + 30;

            ItemShowImage i1_3 = NewImage<ForbiddenLamp>(new Vector2(x1, y1 + 15), conditions: Condition.Hardmode)
                .SetColor(Colors.RarityPink);
            i1_2.AddChainedElement(i1_3);

            x1 += 80 + 30;

            ItemShowImage i1_4 = NewImage<PiezoArmorPanel>(new Vector2(x1 - 80, y1 - 70), conditions: Condition.DownedMartians)
                .SetColor(Colors.RarityYellow);

            ItemShowImage i1_5 = NewImage<SolarTwinkle>(new Vector2(x1, y1), conditions: Condition.DownedSolarPillar)
                .SetColor(Colors.RarityCyan);
            i1_4.AddChainedElement(i1_5);
            i1_3.AddChainedElement(i1_5);

            x1 = -270;
            y1 = 180;

            ItemShowImage i2_0 = NewImage(ItemID.Bone, new Vector2(x1, y1), Readfragment.KnowledgeButtonType.None, Condition.DownedSkeletron)
                .SetColor(Color.White);

            x1 += 70;

            ItemShowImage i2_1 = NewImage<RemainsOfSamurai>(new Vector2(x1, y1 - 10), Readfragment.KnowledgeButtonType.None, Condition.DownedSkeletron)
                .SetColor(Colors.RarityOrange);
            i2_0.AddChainedElement(i2_1);

            x1 += 70;

            ItemShowImage i2_2 = NewImage<OniMask>(new Vector2(x1, y1 + 5), conditions: Condition.DownedSkeletron)
                .SetColor(Colors.RarityOrange);
            i2_1.AddChainedElement(i2_2);

            x1 += 80 + 30;

            ItemShowImage i2_3 = NewImage<KamonFlag>(new Vector2(x1, y1 - 5), conditions: Condition.DownedPlantera)
                .SetColor(Colors.RarityLime);

            x1 -= 90;
            y1 += 120;

            ItemShowImage i2_4 = NewImage(ItemID.Cog, new Vector2(x1, y1), Readfragment.KnowledgeButtonType.None, Condition.DownedMechBossAny)
                .SetColor(Color.White);

            x1 += 70;

            ItemShowImage i2_5 = NewImage<OldClockwork>(new Vector2(x1, y1 - 5), conditions: Condition.DownedMechBossAny)
                .SetColor(Colors.RarityRed);
            i2_4.AddChainedElement(i2_5);

            x1 += 80 + 10;

            ItemShowImage i2_6 = NewImage<SolarPanel>(new Vector2(x1, y1 - 50), conditions: Condition.DownedMartians)
                .SetColor(Colors.RarityYellow);

            x1 += 80 + 20;

            ItemShowImage i2_7 = NewImage<ChronoHeart>(new Vector2(x1, y1 + 10), conditions: Condition.DownedMoonLord)
                .SetColor(Colors.RarityDarkRed);

            i2_5.AddChainedElement(i2_7);
            i2_6.AddChainedElement(i2_7);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, DashSeries, Position.Y + 40, out _);
        }
    }
}
