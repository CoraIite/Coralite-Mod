using Microsoft.Xna.Framework.Graphics;
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
            int y1 = -160;

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, LengthBonusSeries, Position.Y + 40, out _);
        }
    }
}
