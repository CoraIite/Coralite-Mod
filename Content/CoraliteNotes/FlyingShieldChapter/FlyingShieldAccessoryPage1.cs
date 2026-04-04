using Coralite.Content.Items.FlyingShields.Accessories;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldAccessoryPage1 : ItemShowPage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText LengthBonusSeries { get; private set; }
        public static LocalizedText DamageBonusSeries { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            LengthBonusSeries = this.GetLocalization(nameof(LengthBonusSeries));
            DamageBonusSeries = this.GetLocalization(nameof(DamageBonusSeries));
            AddImages();
        }

        public void AddImages()
        {
            //添加上面的一系列
            int x1 = -180;
            int y1 = -60;
            ItemShowImage i1 = NewImage<StretchGlue>(new Vector2(x1-10, y1-80))
                .SetColor(Colors.RarityGreen);
            ItemShowImage i2 = NewImage<FlyingShieldVarnish>(new Vector2(x1-85, y1-40))
                .SetColor(Colors.RarityGreen);
            ItemShowImage i3 = NewImage<FlyingShieldMaintenanceGuide>(new Vector2(x1 - 75, y1+40))
                .SetColor(Colors.RarityGreen);
            ItemShowImage i4 = NewImage<FlyingShieldBattleGuide>(new Vector2(x1, y1 + 80))
                .SetColor(Colors.RarityGreen);

            x1 += 80;
            ItemShowImage i5 = NewImage<FlyingShieldToolbox>(new Vector2(x1, y1))
                .SetColor(Colors.RarityOrange);
            i1.AddChainedElement(i5);
            i2.AddChainedElement(i5);
            i3.AddChainedElement(i5);
            i4.AddChainedElement(i5);
            NewMark(new Vector2(x1-40, y1), ItemShowMark.MarkType.Arrow, Color.LightGreen);

            x1 += 80 + 40;

            ItemShowImage i6 = NewImage<FlyingShieldToolboxProMax>(new Vector2(x1, y1+10),conditions:Condition.Hardmode)
                .SetColor(Colors.RarityYellow);
            i5.AddChainedElement(i6);

            x1 += 80 + 40;
            ItemShowImage i7 = NewImage<NanoAmplifier>(new Vector2(x1, y1 - 10), conditions: Condition.DownedPlantera);
            i6.AddChainedElement(i7);

            //添加下面的一系列
            x1 = -220;
            y1 += 310;
            ItemShowImage i8 = NewImage<HeavyWedges>(new Vector2(x1, y1 - 10))
                .SetColor(Colors.RarityBlue);
            x1 += 80 + 60;

            ItemShowImage i9 = NewImage<FlyingShieldCore>(new Vector2(x1, y1 + 10),conditions:Condition.DownedMechBossAll)
                .SetColor(Colors.RarityPink);
            i8.AddChainedElement(i9);
            x1 += 80 + 60;

            ItemShowImage i10 = NewImage<FlyingShieldTerminalChip>(new Vector2(x1, y1 - 20), conditions: Condition.DownedMartians);
            i9.AddChainedElement(i10);
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
            DrawParaNormal(spriteBatch, DamageBonusSeries, Position.Y + TitleHeight+PageHeight/2, out _);
        }
    }
}
