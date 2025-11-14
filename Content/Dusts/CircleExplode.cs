using Coralite.Core;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class CircleExplode : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            if (dust.fadeIn < 10)
            {
                dust.scale += 0.05f;
            }
            else
            {
                dust.color *= 0.9f;
                dust.scale += 0.02f;
            }

            if (dust.color.A < 10)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.position - Main.screenPosition
                , dust.color, 0, dust.scale);

            return false;
        }
    }
}
