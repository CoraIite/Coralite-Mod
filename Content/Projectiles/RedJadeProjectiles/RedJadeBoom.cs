using Coralite.Core;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.RedJadeProjectiles
{
    public class RedJadeBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 5;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool PreAI()
        {
            return false;
        }
    }
}
