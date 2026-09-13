using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie
{
    public class Rediancie_Beam : ModProjectile
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 3; i++)
                Projectile.SpawnTrailDust(DustID.GemRuby, 0.4f);
        }

        public override void OnKill(int timeLeft)
        {
            // 敌对二次生成改为服务端权威。
            if (!VaultUtils.isClient)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 3; i++)
            {
                Helper.DrawPrettyStarSparkle(1, 0, pos, Coralite.RedJadeRed, Color.DarkRed, 0.5f, 0, 0.5f, 0.5f, 1, Projectile.rotation, new Vector2(2, 1), Vector2.One);
            }
            for (int i = 0; i < 2; i++)
            { 
                Helper.DrawPrettyStarSparkle(1, 0, pos, Color.White, Coralite.RedJadeRed with { A = 0 }, 0.5f, 0, 0.5f, 0.5f, 1, Projectile.rotation, new Vector2(1, 0.5f), Vector2.One);
            }

            return false;
        }
    }
}
