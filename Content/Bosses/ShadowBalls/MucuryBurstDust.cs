using Coralite.Core;
using Coralite.Core.Prefabs.Dusts;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class MucuryBurstDust() : BaseFrameDust(4, 13, 1)
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public override bool Update(Dust dust)
        {
            if (dust.frame.Y < 13)
            {
                //for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = dust.rotation.ToRotationVector2();
                    Dust d = Dust.NewDustPerfect(dust.position + dir.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(12, 30), DustID.SilverCoin, dir * Main.rand.NextFloat(1f, 12f), Scale: Main.rand.NextFloat(1, 3f));

                    //d.noGravity = true;
                }
            }
            base.Update(dust);
            return false;
        }
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
