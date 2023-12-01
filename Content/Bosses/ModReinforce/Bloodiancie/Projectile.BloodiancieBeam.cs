using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public class BloodiancieBeam : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.scale = 1.6f;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            for (int i = 0; i < 4; i++)
                Projectile.SpawnTrailDust(DustID.GemRuby, 0.4f);
            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(DustID.Blood, 0.2f, Scale: Main.rand.NextFloat(1f, 2f));
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
