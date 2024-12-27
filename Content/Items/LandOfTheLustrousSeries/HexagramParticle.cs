using Coralite.Core;
using InnoVault.PRT;
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

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            if (follow != null)
            {
                Position += follow();
            }

            if (Opacity < 5)
            {
                alpha += 1 / 5f;
            }
            else
            {
                distance += Velocity.Length();
                Velocity *= 0.98f;

                if (Opacity > 5 + 14)
                {
                    alpha -= 0.1f;
                    if (Opacity > 6 + 18 + 10)
                    {
                        active = false;
                    }
                }
            }

            Opacity++;
        }

        public static HexagramParticle New(Vector2 center, Vector2 velocity, float rot, float scale, Color newColor = default)
        {
            HexagramParticle p = PRTLoader.PRT_IDToInstances[CoraliteContent.ParticleType<HexagramParticle>()].Clone() as HexagramParticle;

            //设置各种初始值
            p.active = true;
            p.Color = newColor;
            p.Position = center;
            p.Velocity = velocity;
            p.Scale = scale;
            p.Rotation = rot;
            p.SetProperty();

            return p;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Vector2 origin = tex.Size() / 2;
            Color c = Color * alpha;
            for (int i = 0; i < 6; i++)
            {
                float rot = Rotation + (MathHelper.TwoPi / 6 * i);
                Vector2 pos = Position + (rot.ToRotationVector2() * distance) - Main.screenPosition;
                spriteBatch.Draw(tex, pos, null, c * 0.6f, rot
                    , origin, Scale * new Vector2(0.75f, 10f), SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, pos, null, c, rot
                    , origin, Scale * new Vector2(1f, 1f), SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void DrawInUI(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Vector2 origin = tex.Size() / 2;
            Color c = Color * alpha;
            for (int i = 0; i < 6; i++)
            {
                float rot = Rotation + (MathHelper.TwoPi / 6 * i);
                Vector2 pos = Position + (rot.ToRotationVector2() * distance);
                spriteBatch.Draw(tex, pos, null, c * 0.6f, rot
                    , origin, Scale * new Vector2(0.75f, 6f), SpriteEffects.None, 0f);
                spriteBatch.Draw(tex, pos, null, c, rot
                    , origin, Scale * new Vector2(1f, 0.85f), SpriteEffects.None, 0f);
            }
        }
    }
}
