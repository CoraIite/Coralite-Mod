using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
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
            if (Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Owner.Center;

            if (Owner.HeldItem.ModItem is INightmareWeapon)
            {
                Projectile.timeLeft = 2;
            }

            Timer++;
            Projectile.rotation += 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int howMany = 0;
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                howMany = cp.nightmareEnergy;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            float rot = Timer * 0.02f;
            for (int i = 0; i < howMany; i++)
            {
                Vector2 dir = (Projectile.rotation + i * MathHelper.TwoPi / howMany).ToRotationVector2();
                Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos + dir * (36 + factor * 2), Color.White * 0.7f, NightmarePlantera.nightmareRed,
                    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation + rot, new Vector2(1.2f, 1.2f), Vector2.One);
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
