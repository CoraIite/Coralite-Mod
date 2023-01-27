using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Dusts
{
    public class HorizontalStar : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 126, 93);
            dust.fadeIn = 0;
            //dust.position -= new Vector2(63, 46.5f);
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "StarsDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.shader.UseColor(dust.color);

            Vector2 center = dust.position+new Vector2(63,46.5f)*dust.scale;
            if (dust.fadeIn < 10)
            {
                dust.scale *= 1.06f;
                float factor = dust.fadeIn / 10;
                dust.shader.UseOpacity(0.65f - (factor * 0.25f));
                dust.shader.UseSaturation(1.5f + (factor * 0.6f));
            }
            else
            {
                dust.velocity *= 0.9f;
                dust.scale *= 0.9f;
                float factor = dust.fadeIn / 10;
                dust.shader.UseOpacity(0.4f + (factor * 0.25f));
                dust.shader.UseSaturation(2.3f - (factor * 0.6f));
            }

            Lighting.AddLight(center, new Vector3(1f, 0.97f, 0.5f));

            dust.position= center - new Vector2(63, 46.5f) * dust.scale+dust.velocity;
            dust.fadeIn++;

            if (dust.fadeIn > 23)
                dust.active = false;
            return false;
        }
    }
}
