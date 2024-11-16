using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.RedJades
{
    public class RedExplosionParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "LightFog";

        private float scaleAdder;

        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, 256, 256);
            Rotation = Main.rand.NextFloat(6.282f);
            Scale = 0.01f;
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "GlowingDustPass");
            shader.UseOpacity(1);
        }

        public override void Update()
        {
            shader.UseColor(color);
            Scale += scaleAdder;

            if (fadeIn > 8)
            {
                shader.UseOpacity((12f - fadeIn) / 4);
                scaleAdder *= 0.2f;
            }

            fadeIn++;
            if (fadeIn > 12)
                active = false;
        }

        public static void Spawn(Vector2 center, float maxScale, Color newColor = default)
        {
            if (VaultUtils.isServer)
                return;

            RedExplosionParticle particle = NewParticle<RedExplosionParticle>(center, Vector2.Zero, newColor, 0);
            particle.scaleAdder = maxScale / 8;
        }
    }

    public class RedGlowParticle : Particle
    {
        public override string Texture => AssetDirectory.Rediancie + "RedShield_Flow";

        private float scaleAdder;

        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, 256, 256);
            Rotation = Main.rand.NextFloat(6.282f);
            //shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
            //shader.UseOpacity(1);
        }

        public override void Update()
        {
            //shader.UseColor(color);
            Rotation += 0.2f;
            Scale += scaleAdder;
            if (fadeIn > 6)
            {
                //shader.UseOpacity((12f - fadeIn) / 6);
                scaleAdder *= 0.2f;
            }

            if (fadeIn > 8)
                color *= 0.84f;

            fadeIn++;
            if (fadeIn > 14)
                active = false;
        }

        public static void Spawn(Vector2 center, float maxScale, Color newColor = default, float scale = 1)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            RedGlowParticle particle = NewParticle<RedGlowParticle>(center, Vector2.Zero, newColor, scale);
            particle.scaleAdder = (maxScale - scale) / 6;
        }
    }
}
