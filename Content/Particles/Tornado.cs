using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Tornado : ModParticle
    {
        public override string Texture => AssetDirectory.Particles + "Tornado2";
        //public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, Main.rand.Next(8) * 64, 128, 64);
        }

        public override void Update(Particle particle)
        {
            if (particle.fadeIn % 4 == 0)
            {
                particle.frame.Y += 64;
                if (particle.frame.Y > 448)
                    particle.frame.Y = 0;
            }

            if (particle.fadeIn > GetData(particle))
                particle.scale *= 1.09f;
            else
                particle.scale *= 0.975f;

            if (particle.fadeIn < 20)
                particle.color *= 0.92f;

            particle.fadeIn--;
            if (particle.fadeIn < 0)
                particle.active = false;
        }


        public static Particle Spawn(Vector2 center, Vector2 velocity, Color color, float fadeIn, float rotation, float scale = 1f)
        {
            Particle particle = Particle.NewParticleDirect(center, velocity, CoraliteContent.ParticleType<Tornado>(), color, scale);
            particle.fadeIn = fadeIn;
            particle.rotation = rotation + 1.57f;
            particle.datas = new object[1]
            {
                particle.fadeIn-10f,
            };
            return particle;
        }

        public float GetData(Particle particle)
        {
            if (particle.datas is null || particle.datas[0] is not float)
                return -1;

            return (float)particle.datas[0];
        }
    }
}