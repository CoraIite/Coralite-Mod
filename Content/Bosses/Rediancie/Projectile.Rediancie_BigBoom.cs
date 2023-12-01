using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    public class Rediancie_BigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 256;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Helper.RedJadeBigBoom(Projectile.Center);
        }

        public override bool PreAI() => false;
        public override bool PreDraw(ref Color lightColor) => false;

        public override bool CanHitPlayer(Player target)
        {
            return Vector2.Distance(Projectile.Center, target.Center) < Projectile.width / 2;
        }
    }
}
