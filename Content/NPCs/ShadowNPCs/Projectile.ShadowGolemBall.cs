using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.ShadowNPCs
{
    public class ShadowGolemBall : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowProjectiles + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;

            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1800;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.03f;
            Projectile.frameCounter++;

            if (Projectile.frameCounter % 3 == 0)
            {
                Projectile.frame++;
                if (Projectile.frame >= 3)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D maintex = TextureAssets.Projectile[Type].Value;
            Rectangle source = new Rectangle(0, Projectile.frame * maintex.Height / 3, maintex.Width, maintex.Height / 3);
            Vector2 origin = new Vector2(maintex.Width / 2, maintex.Height / 6);

            Main.spriteBatch.Draw(maintex, Projectile.Center - Main.screenPosition, source, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            float direction = Projectile.velocity.ToRotation();
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.Granite, (direction + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.Next(6, 8), 0, default, Main.rand.NextFloat(1.3f, 1.8f));
                dust.noGravity = true;
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            float direction = Projectile.velocity.ToRotation();
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.Granite, (direction + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.Next(6, 8), 0, default, Main.rand.NextFloat(1.3f, 1.8f));
                dust.noGravity = true;
            }

            return base.OnTileCollide(oldVelocity);
        }
    }
}
