using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    public class TileHightlight : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public override void OnSpawn()
        {
            shouldKilledOutScreen = true;
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn > 1)
            {
                fadeIn = 0;

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            var mainTex = TexValue;
            Rectangle frame = mainTex.Frame(3, 3, Frame.X, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
