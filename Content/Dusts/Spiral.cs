using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class Spiral : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 20, 20, 20);
            dust.rotation = Main.rand.NextFloat(6.282f);
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            if (dust.fadeIn > 5)
                dust.color.A = (byte)(dust.color.A*0.94f);

            dust.velocity.X *= 0.99f;
            dust.rotation += dust.velocity.X / 5;
            if (!dust.noGravity && dust.velocity.Y > -4)
                dust.velocity.Y -= 0.05f;

            if (dust.color.A < 10)
                dust.active = false;

            dust.position += dust.velocity;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D mainTex = Texture2D.Value;

            Main.spriteBatch.Draw(mainTex, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates(), dust.color)*(dust.color.A/255f)
                , dust.rotation, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
