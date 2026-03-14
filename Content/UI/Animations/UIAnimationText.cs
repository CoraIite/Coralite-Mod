using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationText(LocalizedText text, Vector2 center, int maxWidth = -1) : UIAnimationComponent(center)
    {
        private Vector2 scale = Vector2.One;

        public override void RecalculateOthers()
        {
            this.SetSize(Helper.GetStringSize(text.Value, scale, maxWidth));
        }

        public override void DrawAnimation(SpriteBatch spriteBatch, int timer,Vector2 center)
        {
            if (timer < StartTime || timer > EndTime)
                return;

            Vector2 pos = GetDimensions().Center();

        }
    }
}
