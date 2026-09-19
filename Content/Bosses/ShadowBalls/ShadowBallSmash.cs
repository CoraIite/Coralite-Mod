using Coralite.Core;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBallSmash : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        ref float State => ref Projectile.ai[0];

        public override void SetDefaults()
        {

        }

        public override void AI()
        {

        }

        public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
        {

            return false;
        }
    }
}
