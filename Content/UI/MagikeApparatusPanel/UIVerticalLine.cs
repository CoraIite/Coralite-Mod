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
            Width.Set(TextureAssets.FishingLine.Width()+6,0);
            Top.Set(0, 0);
            Height.Set(0, 1);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D line = TextureAssets.FishingLine.Value;

            var style = GetDimensions();
            Rectangle frame = new Rectangle(0, 0, line.Width, (int)style.Height);

            spriteBatch.Draw(line, style.Center(), frame, Color.White,0,frame.Size()/2,1,0,0);
        }
    }
}
