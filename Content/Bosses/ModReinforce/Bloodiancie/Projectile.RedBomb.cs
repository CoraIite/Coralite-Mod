using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public class RedBomb : ModProjectile
    {
        public override string Texture => AssetDirectory.Bloodiancie + "RedChocolate";

        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ReadyTimer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.scale = 1.6f;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void AI()
        {
            if (ShootTimer > 0)
            {
                ShootTimer--;
                for (int i = 0; i < 4; i++)
                    Projectile.SpawnTrailDust(DustID.GemRuby, 0.4f);

                Projectile.rotation += 0.2f;

                if (ShootTimer < 1)
                {
                    Projectile.rotation = 0;
                    Projectile.velocity *= 0;
                }

                return;
            }

            float factor = ReadyTimer / Projectile.ai[2];

            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir() * factor * 128, DustID.GemRuby, Vector2.Zero);
                d.noGravity = true;
            }

            if (ReadyTimer > Projectile.ai[2])
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_BigBoom>(),
                    Helper.ScaleValueForDiffMode(40, 45, 50, 50), 8, Projectile.owner);
                Projectile.Kill();
            }

            ReadyTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, 0, 0);
            return false;
        }
    }
}
