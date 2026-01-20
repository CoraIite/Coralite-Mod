using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    public class MagikeLozengeParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "LozengeParticle";

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
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
            MagikeLozengeParticle particle = PRTLoader.NewParticle<MagikeLozengeParticle>(center, Vector2.Zero, color, scale);

            return particle;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            var mainTex = TexValue;
            Rectangle frame = mainTex.Frame(1, 13, 0, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);

            frame = mainTex.Frame(1, 13, 0, 0);
            Color c2 = Color;
            c2.A /= 2;
            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, c2, Rotation, origin, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class MagikeLozengeParticleSPA : Particle
    {
        public override string Texture => AssetDirectory.Particles + "LozengeParticleSPA";

        public float XScale = 1;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Frame.Y++;
            if (Frame.Y > 13)
                active = false;

            if (Frame.Y > 6)
            {
            }
            else
                Scale *= 1.06f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            var mainTex = TexValue;
            Rectangle frame = mainTex.Frame(1, 13, 0, Frame.Y);
            Vector2 origin = frame.Size() / 2;
            Vector2 scale = new Vector2(XScale, 1) * Scale;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Color, Rotation, origin, scale, SpriteEffects.None, 0f);

            //frame = mainTex.Frame(1, 13, 0, 1);
            Color c2 = Color;
            c2.A = 0;
            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, c2, Rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class MagikeLozengeParticle2 : Particle
    {
        public override string Texture => AssetDirectory.Particles + "LozengeParticle2";

        public float recordScale;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity < 7)
            {
                Scale = Helpers.Helper.Lerp(0.25f, recordScale, Opacity / 7);
            }
            else
                Color *= 0.9f;

            if (Color.A < 10 || Opacity > 60)
                active = false;
        }

        public static MagikeLozengeParticle2 Spawn(Vector2 center, Point16 size, Color color)
        {
            float scale = Math.Max(size.X, size.Y) / 2.2f;
            MagikeLozengeParticle2 particle = PRTLoader.NewParticle<MagikeLozengeParticle2>(center, Vector2.Zero, color, 0.25f);
            particle.recordScale = scale;
            return particle;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            var mainTex = TexValue;
            Rectangle frame = mainTex.Frame(2, 1);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);

            frame = mainTex.Frame(2, 1, 1);
            Color c2 = new(255, 255, 255, Color.A / 2);
            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, c2, Rotation, origin, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
