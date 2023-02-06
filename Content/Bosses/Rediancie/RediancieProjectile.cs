using Coralite.Content.Dusts;
using Coralite.Content.Projectiles.RedJadeProjectiles;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    public class Rediancie_Explosion : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 10;
            Projectile.aiStyle = -1;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Helper.PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, Projectile.Center);
            Color red = new Color(221, 50, 50);
            Color grey = new Color(91, 93, 102);
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(6, 6), 0, red, Main.rand.NextFloat(1f, 1.5f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(4, 4), 0, grey, Main.rand.NextFloat(0.8f, 1.2f));
            }
        }

        public override bool PreAI() => false;

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class Rediancie_Strike : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + "RedJadeStrike";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.scale = 2f;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 14)
                Projectile.velocity.Y += 0.08f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, 1f);
                    dust.noGravity = true;
                }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(), lightColor, Projectile.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class Rediancie_Beam : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

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
            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, Projectile.scale);
                    dust.noGravity = true;
                }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
