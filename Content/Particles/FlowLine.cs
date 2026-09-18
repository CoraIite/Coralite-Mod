using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Vectors;
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
        protected float trailWidth;

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
        }

        protected virtual Vector2[] GetDrawPositions() => oldPositions;

        public float TrailWidth(float t) => trailWidth * 2f; //全宽

        public Color TrailColor(float t, float side)
        {
            if (t > 0.5f)
                return Color.Lerp(Color, new Color(0, 0, 0, 0), (t - 0.5f) * 2);

            return Color.Lerp(new Color(0, 0, 0, 0), Color, t * 2);
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => false;

        public override void DrawPrimitive()
        {
            Vector2[] points = GetDrawPositions();
            if (trailStyle == null || points == null)
                return;

            VectorRenderer.DrawStroke(points, trailStyle, new VectorDrawOptions(VectorSpace.World)
            {
                Blend = BlendState.AlphaBlend,
            });
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
                particle.trailWidth = trailWidth;
                particle.trailStyle ??= new StrokeStyle
                {
                    Parameterization = StrokeParameterization.PointIndex,
                    WidthFunction = particle.TrailWidth,
                    ColorFunction = particle.TrailColor,
                };

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
            Vector2[] points = GetDrawPositions();
            if (trailStyle == null || points == null)
                return;

            VectorRenderer.DrawStroke(points, trailStyle, new VectorDrawOptions(VectorSpace.World)
            {
                Blend = BlendState.AlphaBlend,
                Texture = TexValue,
            });
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
                particle.trailWidth = trailWidth;
                particle.trailStyle ??= new StrokeStyle
                {
                    Parameterization = StrokeParameterization.PointIndex,
                    WidthFunction = particle.TrailWidth,
                    ColorFunction = particle.TrailColor,
                };

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
            //旧 TrailPositions 首帧为 null，只分配不填充，保持那一帧画原点
            bool first = poses == null;
            poses ??= new Vector2[oldPositions.Length];
            if (!first)
            {
                Vector2 center = GetCenter();
                for (int i = 0; i < poses.Length; i++)
                    poses[i] = oldPositions[i] + center;
            }
        }

        protected override Vector2[] GetDrawPositions() => poses;

        public static FlowLineThinFollow Spawn(Vector2 center, Vector2 velocity, Func<Vector2> getCenter, float trailWidth, int spawnTime, float rotate, Color color = default)
        {
            if (VaultUtils.isServer)
                return null;

            FlowLineThinFollow particle = PRTLoader.CreateAndInitializePRT<FlowLineThinFollow>(center, velocity, color, 1f);
            if (particle != null)
            {
                particle.Opacity = spawnTime;
                particle.InitializePositionCache(spawnTime);
                particle.trailWidth = trailWidth;
                particle.trailStyle ??= new StrokeStyle
                {
                    Parameterization = StrokeParameterization.PointIndex,
                    WidthFunction = particle.TrailWidth,
                    ColorFunction = particle.TrailColor,
                };

                particle.spawnTime = spawnTime;
                particle.rotate = rotate;
                particle.GetCenter = getCenter;
                particle.updatePosWhenEnd = true;
            }

            return particle;
        }
    }
}
