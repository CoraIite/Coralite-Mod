using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class GlowDust_Prismatic : ModDust
    {
        public override string Texture => AssetDirectory.Sparkles+"EX98";

        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.4f;
            dust.scale *= 0.5f;
        }
        public override bool Update(Dust dust)
        {
            float light = dust.scale;
            Color color = dust.color;
            Vector3 vec = color.ToVector3();
            Lighting.AddLight(dust.position, vec * light);
            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation();
            if (!dust.noGravity)
            {
                dust.velocity *= 0.95f;
                dust.velocity.Y += 0.05f;
                dust.scale *= 0.925f;
            }
            else
            {
                dust.velocity *= 0.9f;
                dust.scale *= 0.925f;
            }
            if (dust.customData != null && dust.customData is Player player)
            {
                dust.position += player.velocity;
            }
            if (dust.customData != null && dust.customData is NPC npc)
            {
                dust.position += npc.velocity;
            }
            if (dust.customData != null && dust.customData is Projectile projectile)
            {
                dust.position += projectile.velocity * (1 + projectile.extraUpdates);
            }
            if (dust.scale < 0.01f) 
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = Texture2D.Value;
            Rectangle rectangle = new(0, 0, texture.Width, texture.Height);
            Color color = dust.color;
            color *= 0.5f;
            int y = 5;
            float OrigScale = dust.scale;
            for (int i = 0; i < y; i++)
            {
                float Scale = OrigScale * (1f + 3f / y * i);
                Vector2 VScale = new(Scale * 1.5f, Scale * 0.3f);
                float Mult = 1f - (float)i / y;
                Color color2 = Color.White * 0.1f;
                color2 *= Mult;
                color *= Mult;
                color.A = 0;
                color2.A = 0;
                Vector2 vector = dust.position - Main.screenPosition;
                if(!dust.noLight)
                    Main.spriteBatch.Draw(texture, vector, rectangle, color2, dust.rotation, Utils.Size(rectangle) / 2f, VScale, 0, 0f);
                Main.spriteBatch.Draw(texture, vector, rectangle, color, dust.rotation, Utils.Size(rectangle) / 2f, VScale, 0, 0f);
            }
            return false;
        }
    }
}
