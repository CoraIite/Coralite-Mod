using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Dusts
{
    public class Slash : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 512, 85);
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D mainTex = Texture2D.Value;
            Main.spriteBatch.Draw(mainTex, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 12), dust.scale, 0, 0);
            return false;
        }

        public override bool Update(Dust dust)
        {
            if (dust.fadeIn % 4 == 0)
            {
                dust.frame.Y += 85;
                if (dust.frame.Y > 85 * 5)
                    dust.active = false;
            }

            dust.fadeIn++;
            return false;
        }
    }
}
