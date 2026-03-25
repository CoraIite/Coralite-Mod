using Coralite.Content.Items.FlyingShields.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldAccessoryPage1 : ItemShowPage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText LengthBonusSeries { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            LengthBonusSeries = this.GetLocalization(nameof(LengthBonusSeries));
            AddImages();
        }

        public void AddImages()
        {
            int x1 = -180;
            int y1 = -80;
            ItemShowImage i1 = NewImage<StretchGlue>(new Vector2(x1-10, y1-80));
            ItemShowImage i2 = NewImage<FlyingShieldVarnish>(new Vector2(x1-85, y1-40));
            ItemShowImage i3 = NewImage<FlyingShieldMaintenanceGuide>(new Vector2(x1 - 75, y1+40));
            ItemShowImage i4 = NewImage<FlyingShieldBattleGuide>(new Vector2(x1, y1 + 80));

            x1 += 80;
            ItemShowImage i5 = NewImage<FlyingShieldToolbox>(new Vector2(x1, y1));
            i1.SetChainedElement(i5);
            i2.SetChainedElement(i5);
            i3.SetChainedElement(i5);
            i4.SetChainedElement(i5);

            x1 += 80+40;

            ItemShowImage i6 = NewImage<FlyingShieldToolboxProMax>(new Vector2(x1, y1+10),conditions:Condition.Hardmode);
            i5.SetChainedElement(i6);

            x1 += 80 + 40;
            ItemShowImage i7 = NewImage<NanoAmplifier>(new Vector2(x1, y1-10), conditions: Condition.DownedPlantera);
            i6.SetChainedElement(i7);
        }

        public override void Recalculate()
        {
#if DEBUG
            ClearImages();
            RemoveAllChildren();

            AddImages();
#endif

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH2(spriteBatch, Title, Color.LightSteelBlue);
            DrawParaNormal(spriteBatch, LengthBonusSeries, Position.Y + TitleHeight, out _);

        }
    }
}
