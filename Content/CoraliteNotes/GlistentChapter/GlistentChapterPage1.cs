using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Glistent;
using Coralite.Content.Items.ShieldPlus;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.GlistentChapter
{
    public class GlistentChapterPage1 : ItemShowPage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
            AddImages();
        }

        public override void AddImages()
        {
            int x = -260;
            int y = 100;
            ItemShowImage i0_0 = NewImage(ItemID.Wood, new Vector2(x, y), Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.Brown);

            x += 70;
            Color leafStoneC = new Color(147, 188, 79);
            ItemShowImage i0_1 = NewImage<LeafStone>(new Vector2(x, y))
                .SetColor(leafStoneC);
            i0_0.AddChainedElement(i0_1);

            //叶石装备
            ItemShowImage i0_2 = NewImage<LeafeoLightArmor>(new Vector2(x, y-190))
                .SetColor(leafStoneC);
            ItemShowImage i0_3 = NewImage<LeafeoHelmet>(new Vector2(x-60, y-120))
                .SetColor(leafStoneC);
            ItemShowImage i0_4 = NewImage<LeafeoBoots>(new Vector2(x + 60, y - 120))
                .SetColor(leafStoneC);
            i0_1.AddChainedElement(i0_2);
            i0_1.AddChainedElement(i0_3);
            i0_1.AddChainedElement(i0_4);

            ItemShowImage i0_5 = NewImage<LeafShield>(new Vector2(x, y + 190))
                .SetColor(leafStoneC);
            i0_1.AddChainedElement(i0_5);


            ItemShowImage i1_1 = NewImage<GlistentBar>(new Vector2(0, y), conditions: CoraliteConditions.DownedRediancie)
               .SetColor(Coralite.GlistentGreen);

            x = 0;

            ItemShowImage i1_2 = NewImage(ItemID.DemoniteBar, new Vector2(-60, y - 50), Readfragment.KnowledgeButtonType.None)
                .SetColor(Coralite.CorruptionPurple);
            ItemShowImage i1_3 = NewImage(ItemID.CrimtaneBar, new Vector2(-10, y - 90), Readfragment.KnowledgeButtonType.None)
                .SetColor(Coralite.CrimsonRed);

            ItemShowImage i1_4 = NewImage(ItemID.Diamond, new Vector2(x - 10, y+ 90), Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.White);

            i0_1.AddChainedElement(i1_1);

            i1_2.AddChainedElement(i1_1);
            i1_3.AddChainedElement(i1_1);
            i1_4.AddChainedElement(i1_1);

            x += 150;

            ItemShowImage i2_1 = NewImage<GlistentHelmet>(new Vector2(x-80, y-200), conditions: CoraliteConditions.DownedRediancie)
               .SetColor(Coralite.GlistentGreen); 
            ItemShowImage i2_2 = NewImage<GlistentBreastplate>(new Vector2(x-10, y-160), conditions: CoraliteConditions.DownedRediancie)
               .SetColor(Coralite.GlistentGreen);
            ItemShowImage i2_3 = NewImage<GlistentLegs>(new Vector2(x+60, y-120), conditions: CoraliteConditions.DownedRediancie)
               .SetColor(Coralite.GlistentGreen);

            i1_1.AddChainedElement(i2_1);
            i1_1.AddChainedElement(i2_2);
            i1_1.AddChainedElement(i2_3);

            ItemShowImage i2_4 = NewImage<GlistentJar>(new Vector2(x -10, y + 160), conditions: CoraliteConditions.DownedRediancie)
               .SetColor(Coralite.GlistentGreen);
            ItemShowImage i2_5 = NewImage<Terranascence>(new Vector2(x + 60, y + 120), conditions: CoraliteConditions.DownedRediancie)
               .SetColor(Coralite.GlistentGreen);
            ItemShowImage i2_6 = NewImage<GreenOnionBurritos>(new Vector2(x + 80, y + 40), conditions: Condition.Hardmode)
               .SetColor(Coralite.GlistentGreen);

            i1_1.AddChainedElement(i2_4);
            i1_1.AddChainedElement(i2_5);
            i1_1.AddChainedElement(i2_6);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title,  Coralite.GlistentGreen);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);
        }
    }
}
