using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class UIVerticalLine : UIElement
    {
        public UIVerticalLine()
        {
            PaddingTop = 12;
            PaddingBottom = 12;
            Width.Set(TextureAssets.FishingLine.Width(),0);
            Height.Set(-12, 1);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D line = TextureAssets.FishingLine.Value;

            var style = GetDimensions();
            Rectangle frame = new Rectangle(0, 0, line.Width, (int)style.Height);

            spriteBatch.Draw(line, style.Position(), frame, Color.White);
        }
    }
}
