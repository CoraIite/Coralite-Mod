using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Dusts
{
    public class WhiteDust:ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.fadeIn = 0;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 6, 6, 6);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.scale *= 0.8f;
            dust.color *= 0.8f;
            dust.fadeIn++;
            if (dust.fadeIn > 4)
                dust.active = false;
            return false;
        }
    }
}