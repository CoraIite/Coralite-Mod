using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ElectricParticle : ModParticle
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "ElectricParticle";

        public override void OnSpawn(Particle particle)
        {
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = Texture2D.Frame(7, 5, 0, Main.rand.Next(5));
            particle.color = Color.White;
        }

        public override void Update(Particle particle)
        {
            particle.fadeIn++;
            particle.center += particle.velocity;
            if (particle.fadeIn > 1 && particle.fadeIn % 4 == 0)
            {
                particle.frame.X += 80;
                if (particle.frame.X > 80 * 6)
                    particle.active = false;
            }
        }
    }

    public class ElectricParticle_Purple : ElectricParticle
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "ElectricParticle_Purple";
    }

    public class ElectricParticle_Follow : ElectricParticle
    {
        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void Update(Particle particle)
        {
            if (!GetCenter(particle, out Vector2 parentCenter))
                return;

            particle.center = parentCenter + particle.velocity;
            particle.fadeIn++;
            if (particle.fadeIn > 1 && particle.fadeIn % 4 == 0)
            {
                particle.frame.X += 80;
                if (particle.frame.X > 80 * 6)
                    particle.active = false;
            }
        }

        public static bool GetCenter(Particle particle, out Vector2 parentCenter)
        {
            if (particle.datas != null && particle.datas[0] is Func<Vector2> func)
            {
                parentCenter = func();
                return true;
            }

            parentCenter = Vector2.Zero;
            particle.active = false;
            return false;
        }

        public static Particle Spawn(Vector2 parentCenter, Vector2 offset, Func<Vector2> GetParentCenter, float scale = 1f)
        {
            Particle p = Particle.NewParticleDirect(parentCenter + offset, offset, CoraliteContent.ParticleType<ElectricParticle_Follow>(), Scale: scale);
            p.datas = new object[1]
            {
                GetParentCenter,
            };

            return p;
        }
    }

    public class LightningParticle : ModParticle
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;

        public override void OnSpawn(Particle particle)
        {
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.frame = Texture2D.Frame(4, 4, 0, Main.rand.Next(4));
            particle.color = Color.White;
        }

        public override void Update(Particle particle)
        {
            particle.fadeIn++;
            particle.center += particle.velocity;
            if (particle.fadeIn > 1 && particle.fadeIn % 5 == 0)
            {
                particle.frame.X += 32;
                if (particle.frame.X > 32 * 3)
                    particle.active = false;
            }
        }
    }

    public class LightningShineBall : ModDust
    {
        public override string Texture => AssetDirectory.Particles + "LightBall";

        public override void OnSpawn(Dust dust)
        {
            dust.color.A = 0;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.fadeIn++;
            if (dust.fadeIn > 5)
                dust.scale *= 0.9f;

            if (dust.fadeIn > 60 || dust.scale < 0.001f)
            {
                dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color, 0, Texture2D.Size() / 2, dust.scale, 0, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color, 0, Texture2D.Size() / 2, dust.scale / 2, 0, 0);
            return false;
        }
    }
}
