using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Particles
{
    public class FlowLine : TrailParticle
    {
        public override string Texture => AssetDirectory.Blank;

        protected int spawnTime;
        protected float rotate;
        /// <summary>
        /// 在运动结束后是否还更新点
        /// </summary>
        protected bool updatePosWhenEnd = false;

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;
        }

        public override void AI()
        {
            if (Opacity < 0)
            {
                Color *= 0.88f;
                if (updatePosWhenEnd)
                {
                    UpdatePositionCache(spawnTime);
                    SetTrailPositions();
                }
            }
            else
            {
                if (Opacity >= spawnTime * 3f / 4f || Opacity < spawnTime / 4f)
                    Velocity = Velocity.RotatedBy(rotate);
                else
                    Velocity = Velocity.RotatedBy(-rotate);

                UpdatePositionCache(spawnTime);
                SetTrailPositions();
            }

            if (Opacity < -120 || Color.A < 10)
                active = false;

            Opacity -= 1f;
            if (Opacity == 0)
                Velocity = Vector2.Zero;
        }

        public virtual void SetTrailPositions()
        {
            trail.TrailPositions = oldPositions;
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => false;

        public override void DrawPrimitive()
        {
            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            EffectLoader.ColorOnlyEffect.World = world;
            EffectLoader.ColorOnlyEffect.View = view;
            EffectLoader.ColorOnlyEffect.Projection = projection;

            trail?.DrawTrail(EffectLoader.ColorOnlyEffect);
        }


        public static void Spawn(Vector2 center, Vector2 velocity, float trailWidth, int spawnTime, float rotate, Color color = default)
        {
            if (VaultUtils.isServer)
                return;

            FlowLine particle = PRTLoader.NewParticle<FlowLine>(center, velocity, color, 1f);
            if (particle != null)
            {
                particle.Opacity = spawnTime;
                particle.InitializePositionCache(spawnTime);
                particle.trail = new Trail(Main.instance.GraphicsDevice, spawnTime, new EmptyMeshGenerator(), factor => trailWidth, factor =>
                {
                    if (factor.X > 0.5f)
                        return Color.Lerp(particle.Color, new Color(0, 0, 0, 0), (factor.X - 0.5f) * 2);

                    return Color.Lerp(new Color(0, 0, 0, 0), particle.Color, factor.X * 2);
                });

                particle.spawnTime = spawnTime;
                particle.rotate = rotate;
            }
        }
    }

    public class FlowLineThin : FlowLine
    {
        public override string Texture => AssetDirectory.Sparkles + "ShotLineSPA";

        public override void DrawPrimitive()
        {
            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            EffectLoader.TextureColorEffect.World = world;
            EffectLoader.TextureColorEffect.View = view;
            EffectLoader.TextureColorEffect.Projection = projection;
            EffectLoader.TextureColorEffect.Texture = TexValue;

            trail?.DrawTrail(EffectLoader.TextureColorEffect);
        }

        public static new void Spawn(Vector2 center, Vector2 velocity, float trailWidth, int spawnTime, float rotate, Color color = default)
        {
            if (VaultUtils.isServer)
                return;

            FlowLineThin particle = PRTLoader.NewParticle<FlowLineThin>(center, velocity, color, 1f);
            if (particle != null)
            {
                particle.Opacity = spawnTime;
                particle.InitializePositionCache(spawnTime);
                particle.trail = new Trail(Main.instance.GraphicsDevice, spawnTime, new EmptyMeshGenerator(), factor => trailWidth, factor =>
                {
                    if (factor.X > 0.5f)
                        return Color.Lerp(particle.Color, new Color(0, 0, 0, 0), (factor.X - 0.5f) * 2);

                    return Color.Lerp(new Color(0, 0, 0, 0), particle.Color, factor.X * 2);
                });

                particle.spawnTime = spawnTime;
                particle.rotate = rotate;
            }
        }
    }

    public class FlowLineThinFollow : FlowLineThin
    {
        public Func<Vector2> GetCenter;
        public Vector2[] poses;

        public override void SetTrailPositions()
        {
            poses ??= new Vector2[oldPositions.Length];
            if (trail.TrailPositions != null)
            {
                Vector2 center = GetCenter();
                for (int i = 0; i < trail.TrailPositions.Length; i++)
                    poses[i] = oldPositions[i] + center;
            }
            trail.TrailPositions = poses;
        }

        public static FlowLineThinFollow Spawn(Vector2 center, Vector2 velocity, Func<Vector2> getCenter, float trailWidth, int spawnTime, float rotate, Color color = default)
        {
            if (VaultUtils.isServer)
                return null;

            FlowLineThinFollow particle = PRTLoader.CreateAndInitializePRT<FlowLineThinFollow>(center, velocity, color, 1f);
            if (particle != null)
            {
                particle.Opacity = spawnTime;
                particle.InitializePositionCache(spawnTime);
                particle.trail = new Trail(Main.instance.GraphicsDevice, spawnTime, new EmptyMeshGenerator(), factor => trailWidth, factor =>
                {
                    if (factor.X > 0.5f)
                        return Color.Lerp(particle.Color, new Color(0, 0, 0, 0), (factor.X - 0.5f) * 2);

                    return Color.Lerp(new Color(0, 0, 0, 0), particle.Color, factor.X * 2);
                });

                particle.spawnTime = spawnTime;
                particle.rotate = rotate;
                particle.GetCenter = getCenter;
                particle.updatePosWhenEnd = true;
            }

            return particle;
        }
    }
}