using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class MucuryBurstParticle() : BaseFrameParticle(4, 13, 2)
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public override void AI()
        {
            if (Frame.Y < 7)
            {
                //for (int i = 0; i < 4; i++)
                if (Main.rand.NextBool(2))
                {
                    Vector2 dir = Rotation.ToRotationVector2();
                    Dust d = Dust.NewDustPerfect(Position + dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(12, 30), DustID.SilverCoin, dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1, 3f));

                    //d.noGravity = true;
                }
            }

            base.AI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            var frameBox = tex.Frame(4, 13,Frame.X, Frame.Y);

            Vector2 origin = new Vector2(0, frameBox.Height);

            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , Color.White, Rotation + MathHelper.PiOver4, origin, Scale, 0, 0);

            return false;
        }
    }
}
