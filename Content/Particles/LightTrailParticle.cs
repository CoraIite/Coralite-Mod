using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LightTrailParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "HorizontalLight";

        private Color targetColor;

        public override void OnSpawn()
        {
            Rotation = Velocity.ToRotation();
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn > 13)
            {
                color = Color.Lerp(color, targetColor, 0.1f);

                if (Velocity.Y < 7)
                {
                    Velocity.Y += 0.1f;
                }

                Velocity.X *= 0.98f;
            }

            if (color.A < 2)
                active = false;

            UpdatePosCachesNormally(oldCenter.Length);
        }

        public static LightTrailParticle Spawn(Vector2 center, Vector2 velocity, Color newcolor, float scale, Color targetColor = default, int trailCacheCount = 10)
        {
            LightTrailParticle p = NewParticle<LightTrailParticle>(center, velocity, newcolor, scale);
            p.targetColor = targetColor == default ? new Color(0, 60, 250, 0) : targetColor;
            p.InitOldCaches(trailCacheCount);

            return p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;
            float scale = Scale;
            Color c = color;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar3 = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar4 = new List<CustomVertexInfo>();

            float height = mainTex.Height * scale;
            int cacheCount = oldCenter.Length;

            for (int i = 0; i < cacheCount; i++)
            {
                float factor = (float)i / cacheCount;
                Vector2 Center = oldCenter[i];
                Vector2 normal = (oldRot[i] + 1.57f).ToRotationVector2();

                Vector2 Top = Center - Main.screenPosition + normal * height;
                Vector2 Bottom = Center - Main.screenPosition - normal * height;

                Vector2 Top2 = Center - Main.screenPosition + normal * height * 1.5f;
                Vector2 Bottom2 = Center - Main.screenPosition - normal * height * 1.5f;

                var Color2 = color;//Color.Lerp(color, Color.DarkBlue, factor);
                bars.Add(new(Top, Color2, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color2, new Vector3(factor, 1, 1)));
                Color2 = Color.White * (c.A / 255f);
                Color2.A = (byte)(Color2.A * 0.3f);
                bar3.Add(new(Bottom2, Color.Transparent, new Vector3(factor, 1, 1)));
                bar3.Add(new(Center - Main.screenPosition, Color2, new Vector3(factor, 0.5f, 1)));
                bar4.Add(new(Center - Main.screenPosition, Color2, new Vector3(factor, 0.5f, 1)));
                bar4.Add(new(Top2, Color.Transparent, new Vector3(factor, 0, 1)));
            }

            spriteBatch.GraphicsDevice.BlendState = BlendState.Additive;
            spriteBatch.GraphicsDevice.Textures[0] = mainTex;
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar3.ToArray(), 0, bar3.Count - 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar4.ToArray(), 0, bar3.Count - 2);
            spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }
}
