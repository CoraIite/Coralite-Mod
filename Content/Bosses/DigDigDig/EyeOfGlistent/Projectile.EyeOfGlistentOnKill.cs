using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.DigDigDig.EyeOfGlistent
{
    public class EyeOfGlistentOnKill : ModProjectile
    {
        public override string Texture => AssetDirectory.GlistentItems + "GlistentBar";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Mythril, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
        }
    }
}
