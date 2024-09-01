using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class HexagramParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "HorizontalLight";

        private float alpha;
        private float distance;

        public Func<Vector2> follow;

        public override bool ShouldUpdateCenter() => false;

        public override void Update()
        {
            if (follow != null)
            {
                Center += follow();
            }

            if (fadeIn < 5)
            {
                alpha += 1 / 5f;
            }
            else
            {
                distance += Velocity.Length();
                Velocity *= 0.98f;

                if (fadeIn > 5 + 14)
                {
                    alpha -= 0.1f;
                    if (fadeIn > 6 + 18 + 10)
                    {
                        active = false;
                    }
                }
            }

            fadeIn++;
        }

        public static HexagramParticle New(Vector2 center, Vector2 velocity, float rot, float scale, Color newColor = default)
        {
            HexagramParticle p = ParticleLoader.GetParticle(CoraliteContent.ParticleType<HexagramParticle>()).NewInstance() as HexagramParticle;

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = scale;
            p.Rotation = rot;
            p.OnSpawn();

            return p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D tex = GetTexture().Value;
            Vector2 origin = tex.Size() / 2;
            Color c = color * alpha;
            for (int i = 0; i < 6; i++)
            {
                float rot = Rotation + (MathHelper.TwoPi / 6 * i);
                Vector2 pos = Center + (rot.ToRotationVector2() * distance) - Main.screenPosition;
                spriteBatch.Draw(tex, pos, null, c * 0.75f, rot
                    , origin, Scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, pos, null, c, rot
                    , origin, Scale * new Vector2(0.9f, 0.5f), SpriteEffects.None, 0f);
            }
        }

        public override void DrawInUI(SpriteBatch spriteBatch)
        {
            Texture2D tex = GetTexture().Value;
            Vector2 origin = tex.Size() / 2;
            Color c = color * alpha;
            for (int i = 0; i < 6; i++)
            {
                float rot = Rotation + (MathHelper.TwoPi / 6 * i);
                Vector2 pos = Center + (rot.ToRotationVector2() * distance);
                spriteBatch.Draw(tex, pos, null, c, rot
                    , origin, Scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, pos, null, c, rot
                    , origin, Scale * new Vector2(0.9f, 0.5f), SpriteEffects.None, 0f);
            }
        }
    }
}
