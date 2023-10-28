using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.RedJades
{
    public class RedExplosionParticle : ModParticle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "LightFog";

        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, 0, 256, 256);
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.scale = 0.01f;
            particle.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
            particle.shader.UseOpacity(1);
        }

        public override void Update(Particle particle)
        {
            particle.shader.UseColor(particle.color);
            particle.scale += particle.oldRot[0];

            if (particle.fadeIn > 8)
            {
                particle.shader.UseOpacity((12f - particle.fadeIn) / 4);
                particle.oldRot[0] *= 0.2f;
            }

            particle.fadeIn++;
            if (particle.fadeIn > 12)
                particle.active = false;
        }

        public static void Spawn(Vector2 center, float maxScale, Color newColor = default)
        {
            Particle particle = Particle.NewParticleDirect(center, Vector2.Zero, CoraliteContent.ParticleType<RedExplosionParticle>(), newColor, 0);
            particle.oldRot = new float[1]
            {
                maxScale/8
            };
        }
    }

    public class RedGlowParticle : ModParticle
    {
        public override string Texture => AssetDirectory.Rediancie + "RedShield_Flow";

        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            particle.frame = new Rectangle(0, 0, 256, 256);
            particle.rotation = Main.rand.NextFloat(6.282f);
            //particle.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
            //particle.shader.UseOpacity(1);
        }

        public override void Update(Particle particle)
        {
            //particle.shader.UseColor(particle.color);
            particle.rotation += 0.2f;
            particle.scale += particle.oldRot[0];
            if (particle.fadeIn > 6)
            {
                //particle.shader.UseOpacity((12f - particle.fadeIn) / 6);
                particle.oldRot[0] *= 0.2f;
            }

            if (particle.fadeIn > 8)
                particle.color *= 0.84f;

            particle.fadeIn++;
            if (particle.fadeIn > 14)
                particle.active = false;
        }

        public static void Spawn(Vector2 center, float maxScale, Color newColor = default, float scale = 1)
        {
            Particle particle = Particle.NewParticleDirect(center, Vector2.Zero, CoraliteContent.ParticleType<RedGlowParticle>(), newColor, scale);
            particle.oldRot = new float[1]
            {
                (maxScale-scale)/6
            };
        }

    }
}
