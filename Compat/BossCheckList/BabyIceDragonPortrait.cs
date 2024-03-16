using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Compat.BossCheckList
{
    public static class BabyIceDragonPortrait
    {
        public static int frameCounter;
        public static int frame;

        public static void DrawPortrait(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Vector2 center = rect.Center();

            float scale = 1f;
            if (Main.zenithWorld)
                scale = 0.4f;

            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.BabyIceDragon + "BabyIceDragon").Value;

            if (++frameCounter > 7)
            {
                frameCounter = 0;
                if (++frame > 3)
                    frame = 0;
            }
            Rectangle frameBox = mainTex.Frame(3, 5, 1, frame);
            Vector2 origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, center, frameBox, color, 0, origin, scale, SpriteEffects.None, 0f);
        }

    }
}
