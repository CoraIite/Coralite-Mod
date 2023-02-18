using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Dusts
{
    public class BulingDust : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + "Buling2";

        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0f;
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.fadeIn = 0;
            //dust.position -= new Vector2(63, 46.5f);
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "StarsDustPass");
            dust.shader.UseOpacity(0.5f);
            dust.shader.UseSaturation(1.5f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            Vector2 center = dust.position + new Vector2(32, 32) * dust.scale;

            if (dust.fadeIn < 8)
                dust.scale *= 1.1f;
            else
                dust.scale *= 0.9f;

            dust.color *= 0.99f;
            dust.shader.UseColor(dust.color);
            Lighting.AddLight(center, new Vector3(1f, 0.97f, 0.5f));

            dust.position = center - new Vector2(32, 32) * dust.scale + dust.velocity;
            dust.fadeIn++;

            if (dust.fadeIn > 15)
                dust.active = false;
            return false;
        }
    }
}
