using Coralite.Content.Items.Steel;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.SteelChapter
{
    public class SteelPage1 : ItemShowPage
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
            int x;
            int y = 100;
            AddBaseItem(y, out ItemShowImage i1_0, out ItemShowImage i1_1);

            x = -260;
            y = -100;
            AddSteel(x, y, i1_0);

            x = 260;
            y = -100;
            AddB9Alloy(x, y, i1_1);

            x = 0;
            y = 180;
            ItemShowImage i4 = NewImage<TwinRocketLauncher>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal, Condition.DownedPlantera)
                .SetColor(Color.Gray);

            i1_0.AddChainedElement(i4);
            i1_1.AddChainedElement(i4);
        }

        private void AddBaseItem(int y, out ItemShowImage i1_0, out ItemShowImage i1_1)
        {
            const int middleLength1 = 40;
            ItemShowImage i0_0 = NewImage(ItemID.IronBar, new Vector2(-middleLength1, y), Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.LightGray);

            ItemShowImage i0_1 = NewImage(ItemID.LeadBar, new Vector2(middleLength1, y), Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.Gray);

            const int middleLength2 = 80;

            i1_0 = NewImage<SteelBar>(new Vector2(-middleLength1 - middleLength2, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            i0_0.AddChainedElement(i1_0);

            i1_1 = NewImage<B9Alloy>(new Vector2(middleLength1 + middleLength2, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            i0_1.AddChainedElement(i1_1);
        }

        private void AddSteel(int x, int y, ItemShowImage i1_0)
        {
            ItemShowImage i2_0 = NewImage<SteelHelmet>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            x += 80;
            ItemShowImage i2_1 = NewImage<SteelCanHead>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            x -= 80;
            y += 80;
            ItemShowImage i2_2 = NewImage<SteelMask>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            x += 80;
            ItemShowImage i2_3 = NewImage<SteelBucketHead>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);

            x -= 40;
            y += 80;
            ItemShowImage i2_4 = NewImage<SteelBreastplate>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            y += 80;
            ItemShowImage i2_5 = NewImage<SteelLegs>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);

            y += 80;
            x -= 40;
            ItemShowImage i2_6 = NewImage<SteelPickaxe>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            y += 80;
            ItemShowImage i2_7 = NewImage<SteelAxe>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            y -= 80;
            x += 80;
            ItemShowImage i2_8 = NewImage<SteelBreaker>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);
            x += 80;
            ItemShowImage i2_9 = NewImage<MedalOfLife>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Colors.RarityRed);
            y += 80;
            ItemShowImage i2_10 = NewImage<CharmOfIsis>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.LightGray);

            i1_0.AddChainedElement(i2_0);
            i1_0.AddChainedElement(i2_1);
            i1_0.AddChainedElement(i2_2);
            i1_0.AddChainedElement(i2_3);
            i1_0.AddChainedElement(i2_4);
            i1_0.AddChainedElement(i2_5);
            i1_0.AddChainedElement(i2_6);
            i1_0.AddChainedElement(i2_7);
            i1_0.AddChainedElement(i2_8);
            i1_0.AddChainedElement(i2_9);

            i2_9.AddChainedElement(i2_10);
        }

        private void AddB9Alloy(int x, int y, ItemShowImage i1_1)
        {
            ItemShowImage i3_0 = NewImage<B9LaserMask>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            y += 80;
            ItemShowImage i3_1 = NewImage<B9MonitorHead>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            y -= 80;
            x -= 80;
            ItemShowImage i3_2 = NewImage<B9SpaceHelmet>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            y += 80;

            ItemShowImage i3_3 = NewImage<B9PlaneHead>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            y -= 80;
            x -= 80;

            ItemShowImage i3_4 = NewImage<B9BatteryHead>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);

            x = 260 - 40;
            y = -100 + 80 + 80;
            ItemShowImage i3_5 = NewImage<B9Breastplate>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            y += 80;
            ItemShowImage i3_6 = NewImage<B9Legs>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            x = 260;
            y += 80;
            ItemShowImage i3_7 = NewImage<B94WindbreakingArrowItem>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            y += 80;
            ItemShowImage i3_8 = NewImage<B94WindbreakingCompoundBow>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);
            x -= 80;
            y -= 80;
            ItemShowImage i3_9 = NewImage<LifePulseDevice>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Colors.RarityRed);
            y += 80;
            ItemShowImage i3_10 = NewImage<OsirisPillar>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Metal)
                .SetColor(Color.Gray);

            i1_1.AddChainedElement(i3_0);
            i1_1.AddChainedElement(i3_1);
            i1_1.AddChainedElement(i3_2);
            i1_1.AddChainedElement(i3_3);
            i1_1.AddChainedElement(i3_4);
            i1_1.AddChainedElement(i3_5);
            i1_1.AddChainedElement(i3_6);
            i1_1.AddChainedElement(i3_7);
            i1_1.AddChainedElement(i3_8);
            i1_1.AddChainedElement(i3_9);

            i3_9.AddChainedElement(i3_10);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, Color.LightGray);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);
        }
    }
}
