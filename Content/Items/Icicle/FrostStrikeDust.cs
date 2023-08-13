using Coralite.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.Icicle
{
    public class FrostStrikeDust : BaseStrikeDust
    {
        public FrostStrikeDust() : base(Coralite.Instance.IcicleCyan, new Color(23, 106, 158), 20) { }

        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, Coralite.Instance.IcicleCyan.ToVector3());
            //float factor = dust.fadeIn / 16f;
            //if (factor < 0.5f)
            //{
            //    dust.scale *= 1.1f;
            //}
            //else
            //    dust.scale -= 0.005f;

            dust.fadeIn++;
            if (dust.fadeIn > 20)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Vector3 light = Main.rgbToHsl(Lighting.GetColor(dust.position.ToTileCoordinates()));
            float factor = dust.fadeIn / MaxTime;
            Helpers.ProjectilesHelper.DrawPrettyLine(light.Z, SpriteEffects.None, dust.position - Main.screenPosition, Highlight, Dark,
                factor, 0, 0.1f, 0.3f, 1f, dust.rotation, dust.scale, new Vector2(2f, 1f));

            Helpers.ProjectilesHelper.DrawPrettyLine(light.Z, SpriteEffects.None, dust.position - Main.screenPosition, Color.White, Color.White * 0.3f,
                factor, 0, 0.1f, 0.3f, 1f, dust.rotation, dust.scale * 0.9f, new Vector2(2f, 1f));

            return false;
        }

    }
}
