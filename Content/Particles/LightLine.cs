using Coralite.Core;
using InnoVault.PRT;
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
        public float maxAlpha = 1;
        public int fadeTime = 15;
        public float scaleY = 0.4f;

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;

            if (follow != null)
                Position += follow();
            if (center != null)
                Position = center();

            Lighting.AddLight(Position, Color.ToVector3() * alpha / 2);

            if (Opacity < fadeTime)
                alpha += maxAlpha / fadeTime;
            else if (alpha > 0)
            {
                alpha -= maxAlpha / fadeTime;
                if (alpha < 0)
                {
                    alpha = 0;
                }
            }

            if (Opacity > fadeTime * 2)
                active = false;
        }

        public static LightLine Spwan(Vector2 center, Vector2 velocity, Color newColor, Func<Vector2> follow = null, float scale = 1, float maxalpha = 1)
        {
            if (VaultUtils.isServer)
            {
                return null;
            }
            LightLine ll = PRTLoader.NewParticle<LightLine>(center, velocity, newColor, scale);
            if (ll != null)
            {
                ll.follow = follow;
                ll.maxAlpha = maxalpha;
            }
            return ll;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            Vector2 origin = new(0, mainTex.Height / 2);
            Color c = Color;
            c.A = (byte)(alpha * 255);

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, null, c, Rotation, origin, new Vector2(Scale, 0.4f), SpriteEffects.None, 0f);
            return false;
        }
    }
}
