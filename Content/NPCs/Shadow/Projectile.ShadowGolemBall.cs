using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.NPCs.Shadow
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
            Projectile.UpdateFrameNormally(3, 4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D maintex = Projectile.GetTexture();
            Rectangle source = new Rectangle(0, Projectile.frame * maintex.Height / 3, maintex.Width, maintex.Height / 3);
            Vector2 origin = new Vector2(maintex.Width / 2, maintex.Height / 6);

            Main.spriteBatch.Draw(maintex, Projectile.Center - Main.screenPosition, source, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
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
