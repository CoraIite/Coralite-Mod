using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.UI.Elements;

namespace Coralite.Content.UI
{
    public class FixedUIGrid : UIGrid
    {
        public FixedUIGrid()
        {
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Recalculate();
        }
    }
}
