using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class TestP : ModParticle, IDrawParticlePrimitive
    {
        BasicEffect effect;

        public override string Texture => AssetDirectory.DefaultItem;

        public TestP()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public void DrawPrimitives(Particle particle)
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            particle.trail?.Render(effect);
        }

        public override void OnSpawn(Particle particle)
        {
            particle.trail = new Trail(Main.instance.GraphicsDevice, 10, new NoTip(), factor => 2, factor => Color.White);
            particle.InitOldCenters(10);
        }

        public override void Update(Particle particle)
        {
            for (int i = 0; i < 9; i++)
            {
                particle.oldCenter[i] = particle.oldCenter[i + 1];
            }

            particle.oldCenter[9] = particle.center;

            particle.trail.Positions = particle.oldCenter;
            particle.fadeIn++;
            if (particle.fadeIn > 30)
                particle.active = false;        
        }
    }
}
