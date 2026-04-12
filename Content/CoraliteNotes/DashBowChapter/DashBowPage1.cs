using Coralite.Content.Items.ThyphionSeries;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.DashBowChapter 
{
    public class DashBowPage1 : ItemShowPage
    {
        public static LocalizedText Arrows { get; private set; }
        public static LocalizedText Accessories { get; private set; }

        public override void OnInitialize()
        {
            Arrows = this.GetLocalization(nameof(Arrows));
            Accessories = this.GetLocalization(nameof(Accessories));
            AddImages();
        }

        public override void AddImages()
        {
            int x = -220;
            int y = -180;

            ItemShowImage i0_1 = NewImage(ItemID.SoulofSight,new Vector2(x, y), Readfragment.KnowledgeButtonType.None, Condition.DownedTwins)
                .SetColor(Color.LimeGreen*1.5f);

            x += 70;

            ItemShowImage i0_2 = NewImage<WindSpeedArrows>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Wild,Condition.DownedTwins)
                .SetColor(Colors.RarityPink);
            i0_1.AddChainedElement(i0_2);

            x += 80+40;

            ItemShowImage i0_3 = NewImage<WindrangerQuiver>(new Vector2(x, y), Readfragment.KnowledgeButtonType.Wild, Condition.DownedPlantera)
                .SetColor(Colors.RarityYellow);
            i0_2.AddChainedElement(i0_3);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //DrawParaNormal(spriteBatch, Arrows, Position.Y + 40, out _);
            DrawParaNormal(spriteBatch, Accessories, Position.Y + 40, out _);
        }
    }
}
