using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using InnoVault.Trails;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class FlowLine : TrailParticle
    {
        public override string Texture => AssetDirectory.Blank;

        private static BasicEffect effect;
        private int spawnTime;
        private float rotate;

        public FlowLine()
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void SetProperty()
        {
        }

        public override void AI()
        {
            if (Opacity < 0)
                Color *= 0.88f;
            else
            {
                if (Opacity >= spawnTime * 3f / 4f || Opacity < spawnTime / 4f)
                    Velocity = Velocity.RotatedBy(rotate);
                else
                    Velocity = Velocity.RotatedBy(-rotate);

                UpdatePositionCache(spawnTime);
                trail.TrailPositions = oldPositions;
            }

            if (Opacity < -120 || Color.A < 10)
                active = false;

            Opacity -= 1f;
            if (Opacity == 0)
                Velocity = Vector2.Zero;

        }

        public override bool PreDraw(SpriteBatch spriteBatch) => false;

        public override void DrawPrimitive()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.DrawTrail(effect);
        }


        public static void Spawn(Vector2 center, Vector2 velocity, float trailWidth, int spawnTime, float rotate, Color color = default)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
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
}