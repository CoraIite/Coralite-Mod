using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmarePetal : ModDust
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(8) * 14, 10, 14);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.2f;
            dust.velocity *= 0.99f;
            if (!dust.noGravity)
                dust.velocity.Y += 0.04f;
            if (dust.fadeIn > 45)
                dust.color *= 0.84f;
            if (dust.fadeIn % 8 == 0)
            {
                dust.frame.Y += 14;
                if (dust.frame.Y > 98)
                    dust.frame.Y = 0;
            }

            dust.fadeIn++;
            if (dust.fadeIn > 60)
                dust.active = false;
            return false;
        }
    }
}