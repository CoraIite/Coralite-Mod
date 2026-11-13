using Coralite.Core;
using Coralite.Core.Prefabs.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class MucuryBurstDust():BaseFrameDust(4,13,2)
    {
        public override string Texture => AssetDirectory.ShadowBalls+Name;
        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = Texture2D.Value;

            var frameBox = tex.Frame(4, 13, dust.frame.X, dust.frame.Y);

            Vector2 origin = new Vector2(0, frameBox.Height);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, frameBox
                , Color.White, dust.rotation+MathHelper.PiOver4, origin, dust.scale, 0, 0);

            return false;
        }
    }
}
