using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    public class MagikeLozengeParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "LozengeParticle";

        public override void OnSpawn()
        {

        }

        public override void Update()
        {
            //fadeIn++;
            //if (fadeIn>1)
            //{
            //    fadeIn = 0;
            Frame.Y++;
            if (Frame.Y > 13)
                active = false;
            //}

            if (Frame.Y > 6)
            {
                //Scale *= 1.03f;

                //color.A = (byte)(color.A * 0.9f);
            }
            else
                Scale *= 1.06f;
        }

        public static MagikeLozengeParticle Spawn(Vector2 center, Point16 size, Color color)
        {
            float scale = Math.Max(size.X, size.Y) / 7f;
            MagikeLozengeParticle particle = NewParticle<MagikeLozengeParticle>(center, Vector2.Zero, color, scale);

            return particle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var mainTex = GetTexture().Value;
            Rectangle frame = mainTex.Frame(1, 13, 0, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Center - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);

            frame = mainTex.Frame(1, 15, 0, 0);
            Color c2 = new(255, 255, 255, color.A / 2);
            spriteBatch.Draw(mainTex, Center - Main.screenPosition, frame, c2, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
