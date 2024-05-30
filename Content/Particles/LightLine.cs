using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LightLine : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "HorizontalLight";

        public Func<Vector2> follow;
        public Func<Vector2> center;

        private float alpha;
        public float maxAlpha=1;
        public int fadeTime = 15;
        public float  scaleY = 0.4f;

        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(6.282f);
        }

        public override void Update()
        {
            fadeIn++;

            if (follow != null)
                Center += follow();
            if (center != null)
                Center = center();
            
            Lighting.AddLight(Center, color.ToVector3() * alpha / 2);

            if (fadeIn < fadeTime)
                alpha += maxAlpha / fadeTime;
            else if (alpha > 0)
            {
                alpha -= maxAlpha / fadeTime;
                if (alpha < 0)
                {
                    alpha = 0;
                }
            }

            if (fadeIn > fadeTime * 2)
                active = false;
        }

        public static LightLine Spwan(Vector2 center, Vector2 velocity, Color newColor, Func<Vector2> follow = null, float scale = 1, float maxalpha = 1)
        {
            LightLine ll = NewParticle<LightLine>(center, velocity, newColor, scale);
            ll.follow = follow;
            ll.maxAlpha = maxalpha;
            return ll;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;
            Vector2 origin = new Vector2(0, mainTex.Height / 2);
            Color c = color;
            c.A = (byte)(alpha * 255);

            spriteBatch.Draw(mainTex, Center - Main.screenPosition, null, c, Rotation, origin, new Vector2(Scale,0.4f), SpriteEffects.None, 0f);
        }
    }
}
