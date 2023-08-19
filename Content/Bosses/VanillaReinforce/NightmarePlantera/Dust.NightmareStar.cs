using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareStar : ModDust
    {
        public override string Texture => AssetDirectory.Blank;

        public override void OnSpawn(Dust dust)
        {

        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;

            if (dust.fadeIn > 14)
            {
                dust.velocity *= 0.98f;
            }

            dust.position += dust.velocity;

            if (dust.fadeIn > 28)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Helpers.ProjectilesHelper.DrawPrettyStarSparkle(1f, 0, dust.position-Main.screenPosition, new Color(50, 50, 50, 100),
                dust.color,dust.fadeIn/28,0,0.1f,0.5f,1f,dust.rotation,new Vector2(dust.scale/2,dust.scale),new Vector2(0.5f,0.5f));
            return false;
        }
    }
}
