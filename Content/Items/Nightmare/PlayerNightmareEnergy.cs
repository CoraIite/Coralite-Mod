using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Nightmare
{
    public class PlayerNightmareEnergy : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override void AI()
        {
            Projectile.Center = Owner.Center;

            if (Owner.HeldItem.ModItem is INightmareWeapon)
            {
                Projectile.timeLeft = 2;
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int howMany = 0;
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                howMany = cp.nightmareEnergy;

            for (int i = 0; i < howMany; i++)
            {

            }
            return false;
        }

        public static void Spawn(Player player, IEntitySource source)
        {
            if (Main.projectile.Any(p => p.active && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<PlayerNightmareEnergy>()))
                return;

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<PlayerNightmareEnergy>(), 1, 0, player.whoAmI);
        }
    }
}
