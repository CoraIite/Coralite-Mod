using Coralite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class PixelPoint:ModDust
    {
        public override string Texture => AssetDirectory.Dusts+Name;

        public override void OnSpawn(Dust dust)
        {
            base.OnSpawn(dust);
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);

            dust.color *= 0.8f;
            dust.velocity *= 0.95f;

            dust.position += dust.velocity;

            if (dust.color.A < 10)
            {
                dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, Texture2D.Frame(2, 1), dust.color, 0, Texture2D.Size() / 2, dust.scale, 0, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, Texture2D.Frame(2, 1, 1), dust.color*2f, 0, Texture2D.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
