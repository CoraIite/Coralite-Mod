using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Compat.BossCheckList
{
    [VaultLoaden(AssetDirectory.SlimeEmperor)]
    public static class SlimeEmperorPortrait
    {
        public static ATex SlimeEmperorPortraitTex { get; set; }

        public static void DrawPortrait(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Vector2 center = rect.Center();

            Color c = color;
            if (Main.zenithWorld)
                c = Color.Lerp(new Color(25, 25, 25, 200), new Color(110, 60, 100, 50),
                    (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) / 2);

            Texture2D mainTex = SlimeEmperorPortraitTex.Value;

            Rectangle frameBox = mainTex.Frame(1, 52, 0, (int)(Main.timeForVisualEffects/2)%52);
            Vector2 origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, center, frameBox, color, 0, origin, 2, SpriteEffects.None, 0f);
        }
    }
}
