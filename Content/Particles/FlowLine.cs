using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class FlowLine : ModParticle, IDrawParticlePrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        BasicEffect effect;

        public FlowLine()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void OnSpawn(Particle particle)
        {
        }

        public override void Update(Particle particle)
        {
            if (particle.fadeIn < 0)
                particle.color *= 0.88f;
            else
            {
                GetDatas(particle, out int spawnTime, out float rotate);
                if (particle.fadeIn >= spawnTime * 3f / 4f || particle.fadeIn < spawnTime / 4f)
                    particle.velocity = particle.velocity.RotatedBy(rotate);
                else
                    particle.velocity = particle.velocity.RotatedBy(-rotate);

                particle.UpdateCachesNormally(spawnTime);
                particle.trail.Positions = particle.oldCenter;
            }

            if (particle.fadeIn < -120 || particle.color.A < 10)
                particle.active = false;

            particle.fadeIn -= 1f;
            if (particle.fadeIn == 0)
                particle.velocity = Vector2.Zero;

        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle) { }

        public void DrawPrimitives(Particle particle)
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            particle.trail?.Render(effect);
        }


        public static Particle Spawn(Vector2 center, Vector2 velocity, float trailWidth, int spawnTime, float rotate, Color color = (default))
        {
            Particle particle = Particle.NewParticleDirect(center, velocity, CoraliteContent.ParticleType<FlowLine>(), color, 1f);
            particle.fadeIn = spawnTime;
            particle.InitOldCenters(spawnTime);
            particle.trail = new Trail(Main.instance.GraphicsDevice, spawnTime, new NoTip(), factor => trailWidth, factor =>
            {
                if (factor.X > 0.5f)
                    return Color.Lerp(particle.color, new Color(0, 0, 0, 0), (factor.X - 0.5f) * 2);

                return Color.Lerp(new Color(0, 0, 0, 0), particle.color, factor.X * 2);
            });

            particle.datas = new object[2]{
                spawnTime,
                rotate
            };

            return particle;
        }

        public static void GetDatas(Particle particle, out int spawnTime, out float rotate)
        {
            if (particle.datas is null || particle.datas[0] is not int)
            {
                spawnTime = 10;
                rotate = 0f;
                return;
            }

            spawnTime = (int)particle.datas[0];
            rotate = (float)particle.datas[1];
        }
    }
}