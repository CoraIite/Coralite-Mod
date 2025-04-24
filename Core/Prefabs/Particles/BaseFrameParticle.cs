using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Prefabs.Particles
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameXCount"></param>
    /// <param name="frameYCount"></param>
    /// <param name="frameCounterMax"></param>
    /// <param name="frameDirVertical">帧图排列方向是否为竖向</param>
    /// <param name="randRot"></param>
    public abstract class BaseFrameParticle(int frameXCount, int frameYCount, int frameCounterMax, bool frameDirVertical = true, bool randRot = false) : Particle
    {
        public SpriteEffects Effects { get; set; }

        public override void SetProperty()
        {
            if (randRot)
                Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            int frameX = 0;
            int frameY = Main.rand.Next(frameYCount);
            if (frameDirVertical)
            {
                frameX = Main.rand.Next(frameXCount);
                frameY = 0;
            }

            Frame = new Rectangle(frameX, frameY, 0, 0);
        }

        public override void AI()
        {
            if (++Opacity>frameCounterMax)
            {
                Opacity = 0;
                if (frameDirVertical)
                {
                    if (++Frame.Y>=frameYCount)
                        active = false;
                }
                else
                {
                    if (++Frame.X >= frameXCount)
                        active = false;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            var frameBox = tex.Frame(frameXCount, frameYCount, Frame.X, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , Lighting.GetColor(Position.ToTileCoordinates(), Color), Rotation, frameBox.Size() / 2, Scale, Effects, 0);

            return false;
        }
    }
}
