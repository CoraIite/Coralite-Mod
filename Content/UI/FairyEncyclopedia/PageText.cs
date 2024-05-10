using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class PageText : UIElement
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderString(spriteBatch, $"{FairyEncyclopedia.PageIndex + 1} / {FairyEncyclopedia.PageCount + 1}"
                , GetDimensions().Center(), Color.White, anchorx: 0.5f, anchory: 0.5f);
        }
    }
}
