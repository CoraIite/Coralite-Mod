using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class GlowDust_Line : ModDust
    {
        public override string Texture => AssetDirectory.Dusts+Name;

        public override void OnSpawn(Dust dust)
        {
            Vector2[] oldPos = new Vector2[10];
            for (int i = 0; i < oldPos.Length; i++)
            {
                oldPos[i] = dust.position;
            }
            dust.customData = oldPos;
        }

        public override bool Update(Dust dust)
        {
            float light = dust.scale;
            Color color = dust.color;
            Vector3 vec = color.ToVector3();
            if(!dust.noLight)
                Lighting.AddLight(dust.position, vec * light);
            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation();
            if (!dust.noGravity)
            {
                dust.velocity *= 0.95f;
                dust.velocity.Y += 0.05f;
                dust.scale *= 0.9f;
            }
            else
            {
                dust.velocity *= 0.9f;
                dust.scale *= 0.9f;
            }
            if (dust.customData is Vector2[] oldPos)
            {
                if (dust.fadeIn < 1)
                {
                    dust.fadeIn = 1;
                    for (int i = 0; i < oldPos.Length; i++)
                    {
                        oldPos[i] = dust.position;
                    }
                }

                for (int i = 0; i < oldPos.Length - 1; i++)
                {
                    oldPos[i] = oldPos[i + 1];
                }
                oldPos[^1] = dust.position;
                dust.customData = oldPos;
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
            if (dust.customData is Vector2[] oldPos)
            {
                for (int i = 1; i < oldPos.Length; i++)
                {
                    Vector2 vector = oldPos[i] - Main.screenPosition;
                    float Rot = (oldPos[i - 1] - oldPos[i]).ToRotation();
                    float Dis = MathHelper.Max(1f, Vector2.Distance(oldPos[i - 1],oldPos[i]));
                    Main.spriteBatch.Draw(texture, vector, rectangle, color, Rot, Utils.Size(rectangle) / 2f, new Vector2(Dis, dust.scale), 0, 0f);
                }
            }
            return false;
        }
    }
}
