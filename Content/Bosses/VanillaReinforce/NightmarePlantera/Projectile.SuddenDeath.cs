using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class SuddenDeath : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 500;
            Projectile.width = Projectile.height = 64;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.timeLeft<5)
            {
                return target.whoAmI == Projectile.owner;
            }

            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.dead)
            {
                Projectile.Kill();
                return;
            }

            owner.Center = Projectile.Center;
            if (Projectile.timeLeft<5&&!owner.dead)
            {
                owner.KillMe(PlayerDeathReason.ByProjectile(Projectile.owner, Projectile.whoAmI), 999999, 0);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
