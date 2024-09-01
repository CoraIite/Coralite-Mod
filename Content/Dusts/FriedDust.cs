using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class FriedDust : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public const int FadeTime = 60 * 2;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(4) * 10, 10, 10);
            dust.rotation = Main.rand.NextFloat(6.282f);
            dust.color = Color.White;
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            dust.rotation += MathF.Sign(dust.velocity.X) * 0.2f;
            if (!dust.noGravity && dust.velocity.Y < 12)
            {
                dust.velocity.Y += 0.1f;
            }

            if (Collision.SolidCollision(dust.position - (Vector2.One * 5f), 10, 10))
            {
                if (dust.fadeIn < FadeTime / 2)
                    dust.fadeIn = FadeTime / 2;
                dust.velocity *= 0.25f;
            }

            if (dust.fadeIn > FadeTime / 2)
            {
                dust.scale *= 0.95f;
                dust.color *= 0.9f;
            }

            dust.position += dust.velocity;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D mainTex = Texture2D.Value;

            Main.spriteBatch.Draw(mainTex, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates()) * (dust.color.A / 255f)
                , dust.rotation, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
