using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Dusts
{
    public abstract class BaseStrikeDust : ModDust
    {
        public override string Texture => AssetDirectory.Blank;

        public readonly Color Highlight;
        public readonly Color Dark;
        public readonly float MaxTime;

        public BaseStrikeDust(Color Highlight, Color Dark, float maxTime)
        {
            this.Highlight = Highlight;
            this.Dark = Dark;
            MaxTime = maxTime;
        }

        public override bool PreDraw(Dust dust)
        {
            Vector3 light = Main.rgbToHsl(Lighting.GetColor(dust.position.ToTileCoordinates()));
            float factor = dust.fadeIn / MaxTime;
            Helpers.Helper.DrawPrettyLine(light.Z, SpriteEffects.None, dust.position - Main.screenPosition, Highlight, Dark,
                factor, 0, 0.1f, 0.5f, 1f, dust.rotation, dust.scale, new Vector2(2f, 1f));

            Helpers.Helper.DrawPrettyLine(light.Z, SpriteEffects.None, dust.position - Main.screenPosition, Color.White, Color.White * 0.3f,
                factor, 0, 0.1f, 0.5f, 1f, dust.rotation, dust.scale * 0.9f, new Vector2(2f, 1f));

            return false;
        }

    }
}
