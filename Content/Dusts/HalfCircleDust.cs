using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Dusts
{
    public class HalfCircleDust : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + "halfCircle";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.fadeIn = 0;
            dust.frame = new Rectangle(0, 0, 32, 32);
            dust.rotation = Main.rand.Next(-3, 4);
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.3f;
            dust.fadeIn++;
            dust.velocity *= 0.92f;
            dust.scale *= 0.92f;
            dust.shader.UseColor(dust.color);

            if (dust.fadeIn > 20)
                dust.active = false;

            return false;
        }
    }
}
