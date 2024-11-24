using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    public class TileHightlight : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            ShouldKillWhenOffScreen = true;
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity > 1)
            {
                Opacity = 0;

                Frame.X++;
                if (Frame.X > 2)
                {
                    Frame.X = 0;
                    Frame.Y++;
                    if (Frame.Y > 2)
                        active = false;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            var mainTex = TexValue;
            Rectangle frame = mainTex.Frame(3, 3, Frame.X, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
