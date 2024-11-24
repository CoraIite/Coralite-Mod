﻿using Coralite.Core;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class WindCircle : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public int frameCounterMax = 1;
        public Vector2 scale = Vector2.One;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }
        public override void AI()
        {
            if (++Opacity > frameCounterMax)
            {
                Opacity = 0;
                if (++Frame.Y > 5)
                    active = false;
            }
        }

        public static void Spawn(Vector2 center, Vector2 velocity, float rotation, Color newcolor, float alpha, float Basescale, Vector2 exScale)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            newcolor.A = (byte)(255 * alpha);
            WindCircle p = PRTLoader.NewParticle<WindCircle>(center, velocity, newcolor, Basescale);
            if (p != null)
            {
                p.Rotation = rotation;
                p.scale = exScale;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Rectangle frame = TexValue.Frame(1, 6, 0, Frame.Y);
            Vector2 origin = new(frame.Width / 2, frame.Height / 2);
            var scale2 = scale * Scale;

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, frame, Color, Rotation, origin, scale2, SpriteEffects.None, 0f);
            Color c = Color;
            c.A = (byte)(0.3f * c.A);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, frame, c, Rotation, origin, scale2 * 1.1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
