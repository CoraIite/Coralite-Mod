using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Fog : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, Main.rand.Next(4) * 64, 64, 64);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            Color *= 0.94f;

            Opacity++;
            if (Opacity > 120 || Color.A < 10)
                active = false;
        }
    }

    public class TwistFog : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public byte fadeInTime = 5;

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(Main.rand.Next(2), Main.rand.Next(2), 2, 2);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;

            if (Opacity < fadeInTime)
                return;

            Velocity *= 0.98f;
            Rotation += 0.1f;
            Scale *= 0.99f;
            Color *= 0.94f;

            if (Opacity > 120 || Color.A < 10)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Color c1 = Color;
            float scale = Scale;

            if (Opacity < fadeInTime)
            {
                c1 *= Opacity / fadeInTime;
                scale *= (0.5f + 0.5f * Opacity / fadeInTime);
            }

            Vector2 pos = Position - Main.screenPosition;
            TexValue.QuickCenteredDraw(spriteBatch, Frame, pos, c1, Rotation, scale);
            c1.A /= 2;
            TexValue.QuickCenteredDraw(spriteBatch, Frame, pos, c1, Rotation, scale);
            return false;
        }
    }

    public class TwistFogDark : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public byte fadeInTime = 5;

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(Main.rand.Next(2), Main.rand.Next(2), 2, 2);
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Opacity++;

            if (Opacity < fadeInTime)
                return;

            Velocity *= 0.98f;
            Rotation += 0.1f;
            Scale *= 0.99f;
            Color *= 0.94f;

            if (Opacity > 120 || Color.A < 10)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Color c1 = Color;
            float scale = Scale;

            if (Opacity < fadeInTime)
            {
                c1 *= Opacity / fadeInTime;
                scale *= (0.5f + 0.5f * Opacity / fadeInTime);
            }

            Vector2 pos = Position - Main.screenPosition;
            TexValue.QuickCenteredDraw(spriteBatch, Frame, pos, c1, Rotation, scale);
            c1.A /= 2;
            TexValue.QuickCenteredDraw(spriteBatch, Frame, pos, c1, Rotation, scale);
            return false;
        }
    }
}
