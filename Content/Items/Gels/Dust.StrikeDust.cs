using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class EmperorSabreStrikeDust : ModDust
    {
        public override string Texture => AssetDirectory.Blank;

        public readonly Color Yellow = new Color(247,245,176);
        public readonly Color Dark = new Color(83,46,85);

        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, 1f, 1f, 0.3f);
            float factor = dust.fadeIn / 20f;
            if (factor < 0.5f)
            {
                dust.scale += 0.4f;
            }
            else
                dust.scale -= 0.15f;

            dust.fadeIn++;
            if (dust.fadeIn > 20)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Vector3 light = Main.rgbToHsl(Lighting.GetColor(dust.position.ToTileCoordinates()));
            float factor = dust.fadeIn / 20f;
            Helpers.ProjectilesHelper.DrawPrettyLine(light.Z, SpriteEffects.None, dust.position - Main.screenPosition, Yellow, Dark,
                factor, 0, 0.1f, 0.5f, 1f, dust.rotation, dust.scale, new Vector2(2f, 1f));

            Helpers.ProjectilesHelper.DrawPrettyLine(light.Z, SpriteEffects.None, dust.position - Main.screenPosition, Color.White, Color.White * 0.3f,
                factor, 0, 0.1f, 0.5f, 1f, dust.rotation, dust.scale * 0.8f, new Vector2(2f, 1f));

            return false;
        }
    }
}
