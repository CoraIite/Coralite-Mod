using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Dusts
{
    public class StarsDust : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.fadeIn = 8;
            dust.velocity *= 0f;
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is Projectile projectile)
            {
                Vector2 vec2 = Vector2.Normalize(dust.position - projectile.Center);
                dust.position = projectile.Center + vec2 * dust.fadeIn;
                dust.rotation = vec2.ToRotation() + 0.785f;
            }
            else if (dust.customData is Player player)
            {
                Vector2 vec2 = Vector2.Normalize(dust.position - player.Center);
                dust.position = player.Center + vec2 * dust.fadeIn;
                dust.rotation = vec2.ToRotation() + 0.785f;
            }

            if (dust.fadeIn < 50)
            {
                dust.fadeIn++;
                dust.scale *= 1.01f;
            }
            else
            {
                dust.scale *= 0.95f;
                dust.color *= 0.99f;
            }

            dust.shader.UseColor(dust.color);

            if (dust.scale < 0.1f)
                dust.active = false;

            return false;
        }
    }
}
