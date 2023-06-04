using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class InvertedShadowBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowProjectiles + Name;
        public override string GlowTexture => AssetDirectory.ShadowProjectiles + Name + "_Glow";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;

            Projectile.extraUpdates = 1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1800;
            Projectile.scale = 0.7f;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.alpha = 120 - (int)(Math.Cos(Projectile.localAI[0] * 0.3f) * 120);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.localAI[0] == 30)
                Projectile.tileCollide = true;

            Projectile.localAI[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float direction = Projectile.velocity.ToRotation();
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.Granite, (direction + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.Next(6, 8), 0, default, Main.rand.NextFloat(1.3f, 1.8f));
                dust.noGravity = true;
                dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.Shadowflame, (direction + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.Next(5, 7), 0, default, Main.rand.NextFloat(1.2f, 1.6f));
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
                dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.Shadowflame, (direction + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.Next(5, 7), 0, default, Main.rand.NextFloat(1.2f, 1.6f));
                dust.noGravity = true;
            }

            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }
    }
}
