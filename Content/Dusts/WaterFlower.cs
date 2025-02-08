using Coralite.Core;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class WaterFlower : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(8);
            dust.fadeIn = 1;
        }

        public override bool Update(Dust dust)
        {
            if (dust.fadeIn % 2 == 0)
            {
                dust.frame.X += dust.frame.Width;
                if (dust.frame.X >= dust.frame.Width * 8)
                    dust.active = false;
            }

            dust.fadeIn++;
            dust.position += dust.velocity;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, dust.color
                , dust.rotation, dust.frame.Size() / 2, dust.scale, 0, 0);

            return false;
        }
    }
}
