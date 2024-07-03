using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class CrystalShine : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "LightGlowShot";

        public float TrailCount = 10;
        public Vector2 scale;
        public Vector2 currentScale;
        public int shotCount;

        private List<float> _shotRotation;
        private List<float> _shotScale;

        public float shineRange = 6;
        public int fadeTime = 15;

        public Func<Vector2> follow;

        public override void Update()
        {
            fadeIn++;

            if (follow != null)
            {
                Center += follow();
            }

            Lighting.AddLight(Center, color.ToVector3() * currentScale.Y * 10);

            int dir = MathF.Sign(Velocity.X);
            for (int i = 0; i < shotCount - 1; i++)
            {
                _shotRotation[i] += Main.rand.NextFloat(0.02f, 0.05f) * dir;
            }

            if (fadeIn > fadeTime)
            {
                Velocity.X *= 0.86f;
                scale.X *= 0.99f;
                scale.Y *= 0.96f;
                currentScale = Vector2.SmoothStep(currentScale, scale, 0.5f);

                color.A = (byte)(color.A * 0.98f);
            }
            else if (fadeIn < 8)
            {
                currentScale = Vector2.SmoothStep(Vector2.Zero, scale, fadeIn / 8);
            }
            else
            {
                Velocity.X *= 0.98f;
            }

            if (currentScale.Y < 0.001f)
                active = false;
        }

        public static CrystalShine Spawn(Vector2 center, Vector2 velocity, int shotCount, Vector2 scale, Color newColor = default)
        {
            var cs = NewParticle<CrystalShine>(center, velocity, newColor);
            cs.shotCount = shotCount;
            cs.scale = scale;

            cs._shotRotation = new List<float>();
            cs._shotScale = new List<float>();

            float randRot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < shotCount - 1; i++)
            {
                cs._shotRotation.Add(randRot + i / (float)shotCount * MathHelper.TwoPi);
                cs._shotScale.Add(Main.rand.NextFloat(0.2f, 1.1f));
            }

            cs._shotRotation.Add(Main.rand.NextFloat(6.282f));
            cs._shotScale.Add(1);

            return cs;
        }

        public static CrystalShine New(Vector2 center, Vector2 velocity, int shotCount, Vector2 scale, Color newColor = default)
        {
            CrystalShine cs = ParticleLoader.GetParticle(CoraliteContent.ParticleType<CrystalShine>()).NewInstance() as CrystalShine;

            //设置各种初始值
            cs.active = true;
            cs.color = newColor;
            cs.Center = center;
            cs.Velocity = velocity;
            cs.Scale = 1;
            cs.OnSpawn();

            cs.shotCount = shotCount;
            cs.scale = scale;

            cs._shotRotation = new List<float>();
            cs._shotScale = new List<float>();

            float randRot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < shotCount - 1; i++)
            {
                cs._shotRotation.Add(randRot + i / (float)shotCount * MathHelper.TwoPi);
                cs._shotScale.Add(Main.rand.NextFloat(0.2f, 1.1f));
            }

            cs._shotRotation.Add(Main.rand.NextFloat(6.282f));
            cs._shotScale.Add(1);

            return cs;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < shotCount; i++)
            {
                DrawShot(spriteBatch, _shotRotation[i], _shotScale[i], Main.screenPosition);
            }
        }

        public override void DrawInUI(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < shotCount; i++)
            {
                DrawShot2(spriteBatch, _shotRotation[i], _shotScale[i], Vector2.Zero);
            }
        }

        public void DrawShot(SpriteBatch spriteBatch, float rot, float exScale, Vector2 screenPos)
        {
            Texture2D mainTex = GetTexture().Value;
            Vector2 pos = Center - screenPos;
            Vector2 origin = new Vector2(0, mainTex.Height / 2);
            Vector2 scale = currentScale * 0.1f * exScale;
            scale.Y *= 2;
            Color c = color;

            spriteBatch.Draw(mainTex, pos
                , null, c, rot, origin, scale, SpriteEffects.None, 0f);
            scale.X *= 1.2f;

            Color c2 = Color.White * (c.A / 255f);
            c2.A = (byte)(c.A * 0.4f);

            spriteBatch.Draw(mainTex, pos
                , null, c2, rot, origin, scale, SpriteEffects.None, 0f);

        }

        public void DrawShot2(SpriteBatch spriteBatch, float rot, float exScale, Vector2 screenPos)
        {
            Texture2D mainTex = GetTexture().Value;
            Color c = color;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar3 = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar4 = new List<CustomVertexInfo>();

            Vector2 dir = rot.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(1.57f);
            Vector2 scale = currentScale * 0.1f * exScale;
            float width = mainTex.Width * scale.X / TrailCount;
            float height = mainTex.Height * scale.Y;

            Vector2 center = Center - screenPos;

            for (int i = 0; i < TrailCount; i++)
            {
                float factor = i / TrailCount;
                Vector2 Center = center + i * dir * width;
                Vector2 Top = Center + normal * height * (1 - factor);
                Vector2 Bottom = Center - normal * height * (1 - factor);

                Vector2 Center2 = center + i * dir * width * 1.2f;
                Vector2 Top2 = Center2 + normal * height * shineRange * (1 - factor);
                Vector2 Bottom2 = Center2 - normal * height * shineRange * (1 - factor);

                bars.Add(new(Top, c, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, c, new Vector3(factor, 1, 1)));
                Color Color2 = Color.White * (c.A / 255f);
                Color2.A = (byte)(c.A * 0.2f);
                bar3.Add(new(Bottom2, Color.Transparent, new Vector3(factor, 1, 1)));
                bar3.Add(new(Center2, Color2, new Vector3(factor, 0.5f, 1)));
                bar4.Add(new(Center2, Color2, new Vector3(factor, 0.5f, 1)));
                bar4.Add(new(Top2, Color.Transparent, new Vector3(factor, 0, 1)));
            }

            spriteBatch.GraphicsDevice.BlendState = BlendState.Additive;
            spriteBatch.GraphicsDevice.Textures[0] = mainTex;
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar3.ToArray(), 0, bar3.Count - 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar4.ToArray(), 0, bar3.Count - 2);
            spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }
}
