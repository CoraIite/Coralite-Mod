using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class SnowdropPetal : ModDust
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(8) * 14, 10, 14);
            dust.color = Color.White;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0.2f, 0.2f, 0.2f));
            dust.position += dust.velocity;
            dust.rotation += 0.2f;
            dust.velocity *= 0.99f;
            dust.velocity.Y += 0.02f;
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