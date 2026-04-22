using Coralite.Content.Items.LandOfTheLustrousSeries.Accessories;
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
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, ShinyBonus, Position.Y + 40, out _);
        }
    }
}
