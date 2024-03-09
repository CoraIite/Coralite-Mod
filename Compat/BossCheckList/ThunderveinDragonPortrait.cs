using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Compat.BossCheckList
{
    public static class ThunderveinDragonPortrait
    {
        public static int frameCounter;
        public static int frame;

        public static void DrawPortrait(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Vector2 center = rect.Center();

            float scale = 1.2f;

            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon").Value;

            if (++frameCounter > 5)
            {
                frameCounter = 0;
                if (++frame > 7)
                    frame = 0;
            }
            Rectangle frameBox = mainTex.Frame(3, 8, 0, frame);
            Vector2 origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, center, frameBox, color, 0, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
