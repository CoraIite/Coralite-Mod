using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBallSmash:ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls+Name;

        ref float State => ref Projectile.ai[0];

        public override void SetDefaults()
        {

        }

        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
    }
}
