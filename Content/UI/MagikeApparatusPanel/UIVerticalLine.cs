using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class UIVerticalLine : UIElement
    {
        public UIVerticalLine()
        {
            PaddingTop = 12;
            PaddingBottom = 12;
        }

        public override void Update(GameTime gameTime)
        {
            if (GetOuterDimensions().Height != Parent.GetDimensions().Height)
            {
                Height.Set(Parent.Height.Pixels, 0);
                Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}
