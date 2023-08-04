using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    public class Rediancie_Strike : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.scale = 1.3f;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Main.rand.Next(2);

            float rot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, rot.ToRotationVector2() * Main.rand.NextFloat(0.5f, 1), Scale: Main.rand.NextFloat(0.8f, 1f));
                rot += MathHelper.TwoPi / 4;
            }
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 12)
                Projectile.velocity.Y += 0.08f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(DustID.GemRuby, 0.4f);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.ai[0] switch
            {
                0 => RediancieFollower.tex1.Value,
                _ => RediancieFollower.tex2.Value
            };
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(), lightColor, Projectile.rotation+1.57f, mainTex.Size()/2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
