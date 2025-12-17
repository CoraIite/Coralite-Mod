using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.RedJades
{
    public class RedExplosionParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "LightFog";

        internal float scaleAdder;

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Frame = new Rectangle(0, 0, 256, 256);
            Rotation = Main.rand.NextFloat(6.282f);
            Scale = 0.01f;
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "GlowingDustPass");
            shader.UseOpacity(1);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            shader.UseColor(Color);
            Scale += scaleAdder;

            if (Opacity > 8)
            {
                shader.UseOpacity((12f - Opacity) / 4);
                scaleAdder *= 0.2f;
            }

            Opacity++;
            if (Opacity > 12)
                active = false;
        }

        public static void Spawn(Vector2 center, float maxScale, Color newColor = default)
        {
            if (VaultUtils.isServer)
                return;

            RedExplosionParticle particle = PRTLoader.NewParticle<RedExplosionParticle>(center, Vector2.Zero, newColor, 0);
            particle.scaleAdder = maxScale / 8;
        }
    }

    public class RedExplosionParticle2 : Particle
    {
        public override string Texture => AssetDirectory.Halos + "StrikeSPA";

        internal float scaleAdder;

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Scale = 0.01f;
            //shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "GlowingDustPass");
            //shader.UseOpacity(1);
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            //shader.UseColor(Color);
            Scale += scaleAdder;

            if (Opacity > 8)
            {
                Color *= 0.84f;
                //shader.UseOpacity((12f - Opacity) / 4);
                scaleAdder *= 0.2f;
            }

            Opacity++;
            if (Opacity > 12)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            float scale = Scale * 2;
            TexValue.QuickCenteredDraw(spriteBatch, Position - Main.screenPosition, Color, Rotation, scale);
            Color light = Color.White*0.9f;
            light *= Color.A / 255f;
            light.A = 0;
            TexValue.QuickCenteredDraw(spriteBatch, Position - Main.screenPosition, light, Rotation, scale);

            return false;
        }

        public static void Spawn(Vector2 center, float maxScale, Color newColor = default)
        {
            if (VaultUtils.isServer)
                return;

            RedExplosionParticle2 particle = PRTLoader.NewParticle<RedExplosionParticle2>(center, Vector2.Zero, newColor, 0);
            particle.scaleAdder = maxScale / 8;
        }
    }

    public class RedGlowParticle : Particle
    {
        public override string Texture => AssetDirectory.Rediancie + "RedShield_Flow";

        internal float scaleAdder;

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Frame = new Rectangle(0, 0, 256, 256);
            Rotation = Main.rand.NextFloat(6.282f);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            //shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
            //shader.UseOpacity(1);
        }

        public override void AI()
        {
            //shader.UseColor(color);
            Rotation += 0.2f;
            Scale += scaleAdder;
            if (Opacity > 6)
            {
                //shader.UseOpacity((12f - fadeIn) / 6);
                scaleAdder *= 0.2f;
            }

            if (Opacity > 8)
                Color *= 0.84f;

            Opacity++;
            if (Opacity > 14)
                active = false;
        }

        public static void Spawn(Vector2 center, float maxScale, Color newColor = default, float scale = 1)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            RedGlowParticle particle = PRTLoader.NewParticle<RedGlowParticle>(center, Vector2.Zero, newColor, scale);
            particle.scaleAdder = (maxScale - scale) / 6;
        }
    }

    public class RedGlowParticle2 : Particle
    {
        public override string Texture => AssetDirectory.Halos + "TwistSPA";

        internal float scaleAdder;
        internal Color darkColor = Color.DarkRed;

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            //shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
            //shader.UseOpacity(1);
        }

        public override void AI()
        {
            //shader.UseColor(color);
            Rotation += 0.1f;
            if (Opacity > 6)
            {
                //shader.UseOpacity((12f - fadeIn) / 6);
                scaleAdder *= 0.2f;
            }

            if (Opacity > 8)
            {
                Color *= 0.84f;
                Scale *= 0.94f;
            }
            else
                Scale += scaleAdder;

            Opacity++;
            if (Opacity > 18)
                active = false;
        }


        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Color dark = darkColor;
            dark *= Color.A / 255f;
            dark.A = 0;

            TexValue.QuickCenteredDraw(spriteBatch, Position - Main.screenPosition, Color, Rotation, Scale);
            TexValue.QuickCenteredDraw(spriteBatch, Position - Main.screenPosition, Color, Rotation, Scale);

            return false;
        }

        public static void Spawn(Vector2 center, float maxScale,Color darkColor, Color newColor = default, float scale = 1)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            RedGlowParticle2 particle = PRTLoader.NewParticle<RedGlowParticle2>(center, Vector2.Zero, newColor, scale);
            particle.scaleAdder = (maxScale - scale) / 6;
            particle.darkColor = darkColor;
        }
    }
}
