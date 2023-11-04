using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Corruption
{
    public class CadaverousDragonHead : ModItem
    {
        public override string Texture => AssetDirectory.CorruptionItems+Name;

        public override void SetDefaults()
        {
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CadaverousDragonHeadProj>()]<1)
            {

            }
        }
    }

    public class CadaverousDragonHeadProj:ModProjectile
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;
    }

    public class CadaverousDragonBreath:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
           Projectile. ArmorPenetration = 15; // Added by TML

            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;

            return false;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            int num = 60;
            int num2 = 12;
            int num3 = num + num2;
            if (Projectile.localAI[0] >= (float)num3)
                Projectile.Kill();

            if (Projectile.localAI[0] >= (float)num)
                Projectile.velocity *= 0.95f;

            bool flag = Projectile.ai[0] == 1f;
            int num4 = 50;
            int num5 = num4;
            if (flag)
            {
                num4 = 0;
                num5 = num;
            }

            if (Projectile.localAI[0] < num5 && Main.rand.NextFloat() < 0.25f)
            {
                short num6 = (flag ? DustID.ShadowbeamStaff : DustID.Shadowflame);
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Projectile.localAI[0], 0f, 72f, 0.5f, 1f), 4, 4, num6, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                if (Main.rand.NextBool(4))
                {
                    dust.noGravity = true;
                    dust.scale *= 3f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                else
                {
                    dust.scale *= 1.5f;
                }

                dust.scale *= 1.5f;
                dust.velocity *= 1.2f;
                dust.velocity += Projectile.velocity * 1f * Utils.Remap(Projectile.localAI[0], 0f, num * 0.75f, 1f, 0.1f) * Utils.Remap(Projectile.localAI[0], 0f, num * 0.1f, 0.1f, 1f);
                dust.customData = 1;
            }

            if (num4 > 0 && Projectile.localAI[0] >= num4 && Main.rand.NextFloat() < 0.5f)
            {
                Vector2 center = Main.player[Projectile.owner].Center;
                Vector2 vector = (Projectile.Center - center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.19634954631328583) * 7f;
                short num7 = 31;
                Dust dust2 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(50f, 50f) - vector * 2f, 4, 4, num7, 0f, 0f, 150, new Color(80, 80, 80));
                dust2.noGravity = true;
                dust2.velocity = vector;
                dust2.scale *= 1.1f + Main.rand.NextFloat() * 0.2f;
                dust2.customData = -0.3f - 0.15f * Main.rand.NextFloat();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int num = (int)Utils.Remap(Projectile.localAI[0], 0f, 72f, 10f, 40f);
            hitbox.Inflate(num, num);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!projHitbox.Intersects(targetHitbox))
                return false;

            return Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0);
        }

    }

    public class CadaverousDragonFireBall
    {

    }
}
