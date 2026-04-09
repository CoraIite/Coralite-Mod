using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldAccessoryPage2 : ItemShowPage
    {
        public static LocalizedText NumberBonusSeries { get; private set; }
        public static LocalizedText ParrySeries { get; private set; }

        public override void OnInitialize()
        {
            NumberBonusSeries = this.GetLocalization(nameof(NumberBonusSeries));
            ParrySeries = this.GetLocalization(nameof(ParrySeries));
            AddImages();
        }

        public override void AddImages()
        {
            //添加上面的一系列
            int x1 = -260;
            int y1 = -120;

            ItemShowImage i0_0 = NewImage(ItemID.Leather,new Vector2(x1, y1 + 40),Readfragment.KnowledgeButtonType.None)
                .SetColor(Coralite.CorruptionPurple);
            ItemShowImage i0_1 = NewImage(ItemID.Vertebrae,new Vector2(x1, y1 - 40),Readfragment.KnowledgeButtonType.None)
                .SetColor(Coralite.CrimsonRed);

            x1 += 80;

            ItemShowImage i1 = NewImage<ShieldbearersBand>(new Vector2(x1, y1 + 10))
                .SetColor(Colors.RarityGreen);
            i0_0.AddChainedElement(i1);
            i0_1.AddChainedElement(i1);
            x1 += 80 + 60;

            ItemShowImage i2 = NewImage<PowerliftExoskeleton>(new Vector2(x1, y1 - 10), conditions: Condition.DownedMechBossAny)
                .SetColor(Colors.RarityPink);
            i1.AddChainedElement(i2);
            x1 += 80 + 60;

            ItemShowImage i3 = NewImage<BeetleLimbStrap>(new Vector2(x1, y1 + 20), conditions: Condition.DownedGolem);
            i2.AddChainedElement(i3);


            //下面的系列
             x1 = -200;
             y1 += 330;

            ItemShowImage i1_1 = NewImage<JungleTurtleShell>(new Vector2(x1, y1 - 80))
                .SetColor(Colors.RarityGreen);
            ItemShowImage i1_2 = NewImage<SnowflakeCharm>(new Vector2(x1 - 60, y1))
                .SetColor(Colors.RarityGreen);
            ItemShowImage i1_3 = NewImage<AmberAmulet>(new Vector2(x1 + 20, y1 + 60))
                .SetColor(Colors.RarityOrange);

            x1 += 120;
            ItemShowImage i1_4 = NewImage<DemonsProtection>(new Vector2(x1, y1 -20),conditions:Condition.DownedSkeletron)
                .SetColor(Colors.RarityRed);
            i1_1.AddChainedElement(i1_4);
            i1_2.AddChainedElement(i1_4);
            i1_3.AddChainedElement(i1_4);

            x1 += 40;

            ItemShowImage i1_5 = NewImage<HolyCharm>(new Vector2(x1+10, y1 + 80), conditions:Condition.DownedMechBossAny)
                .SetColor(Colors.RarityPink);

            x1 += 40;

            ItemShowImage i1_6 = NewImage<RustedShield>(new Vector2(x1, y1 - 80),Readfragment.KnowledgeButtonType.None, conditions:Condition.DownedPlantera)
                .SetColor(Colors.RarityYellow);

            x1 += 80;

            ItemShowImage i1_7 = NewImage<Terracrest>(new Vector2(x1, y1), conditions:Condition.DownedPlantera)
                .SetColor(Colors.RarityYellow);
            i1_4.AddChainedElement(i1_7);
            i1_5.AddChainedElement(i1_7);
            i1_6.AddChainedElement(i1_7);

            NewImage<EtheriaLegacy>(new Vector2(x1-5, y1-90), conditions: Condition.DownedOldOnesArmyT2);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, NumberBonusSeries, Position.Y + 40, out _);
            DrawParaNormal(spriteBatch, ParrySeries, Position.Y + PageHeight *0.45f + 40, out _);
        }
    }
}
