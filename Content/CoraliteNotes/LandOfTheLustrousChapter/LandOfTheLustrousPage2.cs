using Coralite.Content.Items.LandOfTheLustrousSeries.Accessories;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    public class LandOfTheLustrousPage2 : ItemShowPage
    {
        public static LocalizedText ShinyBonus { get; private set; }

        public override void OnInitialize()
        {
            ShinyBonus = this.GetLocalization(nameof(ShinyBonus));
            AddImages();
        }

        public override void AddImages()
        {
            int y = -40;

            ItemShowImage i1 = NewImage<EightsquareHand>(new Vector2(-70, y))
                .SetColor(Colors.RarityOrange);
            ItemShowImage i2 = NewImage<VioletEmblem>(new Vector2(70, y), conditions: Condition.DownedPirates)
                .SetColor(Colors.RarityLime);
            i1.AddChainedElement(i2);

            y += 100;

            ItemShowImage i3 = NewImage<SmokyRing>(new Vector2(-70, y), conditions: CoraliteConditions.DownedRediancie)
                .SetColor(Colors.RarityBlue);

            ItemShowImage i4_0 = NewImage(ItemID.ManaFlower, new Vector2(50, y), Readfragment.KnowledgeButtonType.None)
                .SetColor(Colors.RarityBlue);
            ItemShowImage i4_1 = NewImage<FullmoonFlower>(new Vector2(120, y), conditions: Condition.DownedPlantera)
                .SetColor(Colors.RarityYellow);

            i4_0.AddChainedElement(i4_1);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, ShinyBonus, Position.Y + 40, out _);
        }
    }
}
