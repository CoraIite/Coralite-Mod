﻿using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LightShotParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "LightGlowShot";
        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn > 15)
            {
                color = Color.Lerp(color, new Color(0, 60, 200, 0), 0.15f);
                Velocity.X *= 0.86f;
            }
            else if (fadeIn < 4)
            {
                Velocity *= 1.2f;
            }
            else
            {
                Velocity.X *= 0.98f;
            }

            if (color.A < 2)
                active = false;
        }

        /// <summary>
        /// 使用速度来充当缩放，虽然不太好但先就这样
        /// </summary>
        /// <param name="center"></param>
        /// <param name="newcolor"></param>
        /// <param name="rotation"></param>
        /// <param name="circleScale"></param>
        /// <returns></returns>
        public static Particle Spawn(Vector2 center, Color newcolor, float rotation, Vector2 circleScale)
        {
            Particle p = NewParticle<LightShotParticle>(center, Vector2.Zero, newcolor, 1);

            p.Rotation = rotation;
            p.Velocity = circleScale;
            return p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;
            Vector2 pos = Center - Main.screenPosition;
            Vector2 origin = new Vector2(0, mainTex.Height / 2);
            Vector2 scale = Velocity * 0.3f;
            scale.Y *= 2;
            Color c = color;

            spriteBatch.Draw(mainTex, pos
                , null, c, Rotation, origin, scale, SpriteEffects.None, 0f);
            scale.X *= 1.2f;

            Color c2 = Color.White * (c.A / 255f);
            c2.A = (byte)(c.A * 0.4f);

            spriteBatch.Draw(mainTex, pos
                , null, c2, Rotation, origin, scale, SpriteEffects.None, 0f);
            //c = Color.White * (c.A / 255f);

            //List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            //List<CustomVertexInfo> bar3 = new List<CustomVertexInfo>();
            //List<CustomVertexInfo> bar4 = new List<CustomVertexInfo>();

            //Vector2 dir = Rotation.ToRotationVector2();
            //Vector2 normal = dir.RotatedBy(1.57f);
            //float width = mainTex.Width * scale.X / 15;
            //float height = mainTex.Height * scale.Y;

            //for (int i = 0; i < 15; i++)
            //{
            //    float factor = i / 15f;
            //    Vector2 Center = this.Center + i * dir * width;
            //    Vector2 Top = Center - Main.screenPosition + normal * height * (1 - factor);
            //    Vector2 Bottom = Center - Main.screenPosition - normal * height * (1 - factor);

            //    Vector2 Top2 = Center - Main.screenPosition + normal * height * 6 * (1 - factor);
            //    Vector2 Bottom2 = Center - Main.screenPosition - normal * height * 6 * (1 - factor);

            //    var Color2 = Color.Lerp(color, Color.DarkBlue, factor);
            //    bars.Add(new(Top, Color2, new Vector3(factor, 0, 1)));
            //    bars.Add(new(Bottom, Color2, new Vector3(factor, 1, 1)));
            //Color2.A = (byte)(c.A * 0.2f);
            //bar2.Add(new(Top, Color2, new Vector3(factor, 0, 1)));
            //bar2.Add(new(Bottom, Color2, new Vector3(factor, 1, 1)));
            //Color2 = Color.White * (c.A / 255f);
            //Color2.A = (byte)(c.A * 0.4f);
            //bar3.Add(new(Bottom2, Color.Transparent, new Vector3(factor, 1, 1)));
            //bar3.Add(new(Center - Main.screenPosition, Color2, new Vector3(factor, 0.5f, 1)));
            //bar4.Add(new(Center - Main.screenPosition, Color2, new Vector3(factor, 0.5f, 1)));
            //bar4.Add(new(Top2, Color.Transparent, new Vector3(factor, 0, 1)));
            //}

            //spriteBatch.GraphicsDevice.BlendState = BlendState.Additive;
            //spriteBatch.GraphicsDevice.Textures[0] = mainTex;
            //spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            //spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar3.ToArray(), 0, bar3.Count - 2);
            //spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar4.ToArray(), 0, bar3.Count - 2);
            //spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }
}
