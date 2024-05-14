using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LightTrailParticle : ModParticle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "HorizontalLight";

        public override void OnSpawn(Particle particle)
        {
            particle.rotation = particle.velocity.ToRotation();
            particle.InitOldCaches(10);
        }

        public override void Update(Particle particle)
        {
            particle.fadeIn++;
            if (particle.fadeIn > 13)
            {
                particle.color = Color.Lerp(particle.color, new Color(0, 60, 250, 0), 0.1f);

                if (particle.velocity.Y < 7)
                {
                    particle.velocity.Y += 0.1f;
                }

                particle.velocity.X *= 0.98f;
            }

            if (particle.color.A < 2)
                particle.active = false;

            particle.UpdatePosCachesNormally(10);
        }

        public static Particle Spawn(Vector2 center, Vector2 velocity, Color newcolor, float scale)
        {
            Particle p = Particle.NewParticleDirect(center, velocity, CoraliteContent.ParticleType<LightTrailParticle>()
                  , newcolor, scale);
            return p;
        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
            Texture2D mainTex = modParticle.Texture2D.Value;
            float scale = particle.scale;
            Color c = particle.color;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar3 = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar4 = new List<CustomVertexInfo>();

            float height = mainTex.Height * scale;

            for (int i = 0; i < 10; i++)
            {
                float factor = i / 10f;
                Vector2 Center = particle.oldCenter[i];
                Vector2 normal = (particle.oldRot[i] + 1.57f).ToRotationVector2();

                Vector2 Top = Center - Main.screenPosition + normal * height;
                Vector2 Bottom = Center - Main.screenPosition - normal * height;

                Vector2 Top2 = Center - Main.screenPosition + normal * height * 1.5f;
                Vector2 Bottom2 = Center - Main.screenPosition - normal * height * 1.5f;

                var Color2 = particle.color;//Color.Lerp(particle.color, Color.DarkBlue, factor);
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
