using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareDust:ModDust
    {
        public override string Texture => AssetDirectory.NightmarePlantera+Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Microsoft.Xna.Framework.Rectangle(0, Main.rand.Next(0, 5) * 8, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            dust.scale *= 0.99f;
            if (dust.scale<0.1f)
                dust.active = false;

            if (!dust.noGravity)
            {
                dust.velocity.Y += 0.25f;
                if (dust.velocity.Y>16)
                    dust.velocity.Y = 16;
            }

            dust.rotation += 0.25f;
            dust.position += dust.velocity;

            dust.fadeIn++;
            if (dust.fadeIn>80)
                dust.active = false;

            return false;
        }
    }
}
