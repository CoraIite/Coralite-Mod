using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
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

        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            if (fadeIn < 0)
                color *= 0.88f;
            else
            {
                if (fadeIn >= spawnTime * 3f / 4f || fadeIn < spawnTime / 4f)
                    Velocity = Velocity.RotatedBy(rotate);
                else
                    Velocity = Velocity.RotatedBy(-rotate);

                UpdatePositionCache(spawnTime);
                trail.Positions = oldPositions;
            }

            if (fadeIn < -120 || color.A < 10)
                active = false;

            fadeIn -= 1f;
            if (fadeIn == 0)
                Velocity = Vector2.Zero;

        }

        public override void Draw(SpriteBatch spriteBatch) { }

        public override void DrawPrimitives()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }


        public static void Spawn(Vector2 center, Vector2 velocity, float trailWidth, int spawnTime, float rotate, Color color = default)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            FlowLine particle = NewParticle<FlowLine>(center, velocity, color, 1f);
            if (particle != null)
            {
                particle.fadeIn = spawnTime;
                particle.InitializePositionCache(spawnTime);
                particle.trail = new Trail(Main.instance.GraphicsDevice, spawnTime, new NoTip(), factor => trailWidth, factor =>
                {
                    if (factor.X > 0.5f)
                        return Color.Lerp(particle.color, new Color(0, 0, 0, 0), (factor.X - 0.5f) * 2);

                    return Color.Lerp(new Color(0, 0, 0, 0), particle.color, factor.X * 2);
                });

                particle.spawnTime = spawnTime;
                particle.rotate = rotate;
            }
        }
    }
}