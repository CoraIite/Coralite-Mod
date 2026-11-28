using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class CrossLightParticle : Particle
    {
        public int maxTime = Main.rand.Next(8, 15);

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Opacity++;

            if (Opacity > maxTime)
            {
                active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 3; i++)
            {
                Helper.DrawPrettyStarSparkle(1, 0, Position - Main.screenPosition, Color with { A = 0 } * 0.6f, Color * 0.7f, Opacity / maxTime, 0, 0.4f, 0.6f, 1, Rotation, Vector2.One * Scale * (1 - i * 0.15f), new Vector2(0.6f + i / 3 * 0.4f, 1-i*0.15f));
            }

            return false;
        }
    }
}
