using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.Icicle;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldAccessoryPage4 : ItemShowPage
    {
        public static LocalizedText LengthBonusSeries { get; private set; }

        public override void OnInitialize()
        {
            LengthBonusSeries = this.GetLocalization(nameof(LengthBonusSeries));
            AddImages();
        }

        public override void AddImages()
        {
            int x1 = -270;
            int y1 = -140;

            ItemShowImage i0_1 = NewImage(ItemID.ShadowScale, new Vector2(x1, y1 + 60), Readfragment.KnowledgeButtonType.None, Condition.DownedEaterOfWorlds)
                .SetColor(Coralite.CorruptionPurple);
            ItemShowImage i0_2 = NewImage<IcicleBreath>(new Vector2(x1, y1 ), Readfragment.KnowledgeButtonType.None, conditions: Condition.DownedMechBossAny)
                .SetColor(Coralite.IcicleCyan);
            ItemShowImage i0_3 = NewImage(ItemID.TissueSample, new Vector2(x1, y1 - 60), Readfragment.KnowledgeButtonType.None, Condition.DownedBrainOfCthulhu)
                .SetColor(Coralite.CrimsonRed);

            x1 += 70;
            ItemShowImage i1 = NewImage<LevitationBlossom>( new Vector2(x1, y1),  conditions:Condition.DownedEowOrBoc)
                .SetColor(Colors.RarityGreen);
            i0_1.AddChainedElement(i1);
            i0_2.AddChainedElement(i1);
            i0_3.AddChainedElement(i1);

            x1 += 80;
            ItemShowImage i2 = NewImage<ChlorophyteMedal>(new Vector2(x1, y1 + 60), conditions: Condition.DownedMechBossAny)
                .SetColor(Colors.RarityLime);
            x1 += 100;

            ItemShowImage i3 = NewImage<FloralHarmonyMedallion>(new Vector2(x1, y1-20), conditions: Condition.DownedGolem)
                .SetColor(Colors.RarityYellow);
            i1.AddChainedElement(i3);
            i2.AddChainedElement(i3);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, LengthBonusSeries, Position.Y + 40, out _);
        }
    }
}
