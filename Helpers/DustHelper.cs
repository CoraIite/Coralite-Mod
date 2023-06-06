using Microsoft.Xna.Framework;
using Terraria;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core;
using Coralite.Content.Particles;
using Terraria.ID;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        public static void RedJadeExplosion(Vector2 center, bool canMakeSound = true)
        {
            if (canMakeSound)
                PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, center);

            Color red = new Color(221, 50, 50);
            int type = CoraliteContent.ParticleType<LightBall>();

            for (int i = 0; i < 2; i++)
            {
                Particle.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(16, 18), type, red, Main.rand.NextFloat(0.1f, 0.15f));
            }
            for (int i = 0; i < 5; i++)
            {
                Particle.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(10, 14), type, red, Main.rand.NextFloat(0.1f, 0.15f));
                Particle.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(10, 14), type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));
                Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, NextVec2Dir() * Main.rand.NextFloat(4, 8),Scale: Main.rand.NextFloat(1.6f, 1.8f));
                dust.noGravity = true;
            }

            Content.Items.RedJades.RedExplosionParticle.Spawn(center, 0.4f, Coralite.Instance.RedJadeRed);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.35f, Coralite.Instance.RedJadeRed,0.2f);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.35f, Coralite.Instance.RedJadeRed, 0.2f);

        }

        public static void RedJadeBigBoom(Vector2 center,bool canMakeSound=true)
        {
            if (canMakeSound)
                PlayPitched("RedJade/RedJadeBoom", 0.8f, -1f, center);

            Color red = new Color(221, 50, 50);
            int type = CoraliteContent.ParticleType<LightBall>();

            for (int i = 0; i < 4; i++)
            {
                Particle.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(32, 34), type, red, Main.rand.NextFloat(0.15f, 0.2f));
            }
            for (int i = 0; i < 10; i++)
            {
                Particle.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(18, 26), type, red, Main.rand.NextFloat(0.1f, 0.15f));
                Particle.NewParticle(center, NextVec2Dir() * Main.rand.NextFloat(18, 26), type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));
                Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, NextVec2Dir() * Main.rand.NextFloat(6, 10),Scale:Main.rand.NextFloat(2f,2.4f));
                dust.noGravity = true;
            }

            Content.Items.RedJades.RedExplosionParticle.Spawn(center, 0.9f, Coralite.Instance.RedJadeRed);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.8f, Coralite.Instance.RedJadeRed, 0.4f);
            Content.Items.RedJades.RedGlowParticle.Spawn(center, 0.8f, Coralite.Instance.RedJadeRed, 0.4f);

            var modifier = new PunchCameraModifier(center, NextVec2Dir(), 6, 4f, 14, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
        }

    }
}
