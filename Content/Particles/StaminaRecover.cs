using Coralite.Core;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class StaminaRecover : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + "WindCircle";
        public int frameCounterMax = 1;
        public Vector2 scale = Vector2.One;
        public int ownerIndex;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            if (++Opacity > frameCounterMax)
            {
                Opacity = 0;
                if (++Frame.Y > 5)
                    active = false;
            }

            Color *= 0.8f;
            Scale*= 1.04f;
            Position += Main.player[ownerIndex].position - Main.player[ownerIndex].oldPosition;
        }

        public static void Spawn(Vector2 center, Vector2 velocity, float rotation, Color newcolor, float alpha, float Basescale, Vector2 exScale,int ownerIndex)
        {
            if (VaultUtils.isServer)
                return;

            newcolor.A = (byte)(255 * alpha);
            StaminaRecover p = PRTLoader.NewParticle<StaminaRecover>(center, velocity, newcolor, Basescale);
            if (p != null)
            {
                p.Rotation = rotation;
                p.scale = exScale;
                p.ownerIndex = ownerIndex;
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
