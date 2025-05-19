using Coralite.Core;
using Coralite.Core.Prefabs.Misc;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Bosses.DigDigDig.Stonelime
{
    public class StonelimeBossBar : BaseBossHealthBar
    {
        public override string Texture => AssetDirectory.DigDigDigBoss + Name;

        public override int HealthBarFrameWidth => 48;

        public override Point BarSize => new(412, 20);

        public override void DrawBar(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle barFrame, Color barColor, Vector2 stretchScale,NPC npc )
        {
            barTopLeft += offset;

            int width = (int)(stretchScale.X * (barFrame.Width + 2));

            while (width > 0)
            {
                int width2 = barFrame.Width;
                if (width2 > width)
                    width2 = width;

                barFrame.Width = width2;
                spriteBatch.Draw(barTexture, barTopLeft, barFrame, barColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                barTopLeft.X += width2 - 2;

                width -= barFrame.Width;
            }

            //base.DrawBar(spriteBatch, barTexture, barTopLeft-offset, barFrame,barColor, stretchScale);
        }
    }
}
