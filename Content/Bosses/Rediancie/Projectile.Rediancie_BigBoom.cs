using Coralite.Core;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie
{
    public class Rediancie_BigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 256;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override bool PreAI()
        {
            if (!span)
            {
                Helper.RedJadeBigBoom(Projectile.Center);
                span = true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override bool CanHitPlayer(Player target)
        {
            return Vector2.Distance(Projectile.Center, target.Center) < Projectile.width / 2;
        }
    }
}
