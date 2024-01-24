using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    public class RedPulse : ModProjectile
    {
        public override string Texture => AssetDirectory.Rediancie + "RedBink_BossMinion";

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.scale = 0.8f;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(DustID.GemRuby, 0.4f, Scale: 1.4f);

            if (Timer % 10 == 0 && Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2CircularEdge(32, 32), Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, (int)Timer % 30);

            if (Timer % 2 == 0)
                foreach (var player in Main.player.Where(p => p.active && Vector2.Distance(p.Center, Projectile.Center) < 100))
                    Projectile.Kill();

            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_BigBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

    }
}
