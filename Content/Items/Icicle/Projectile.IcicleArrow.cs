using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;

            Projectile.coldDamage = true;
            Projectile.friendly = true;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.velocity.Y += 0.05f;
            if (Projectile.velocity.Y > 12)
                Projectile.velocity.Y = 12;

            if (Main.rand.NextBool())
            {
                Projectile.SpawnTrailDust(DustID.IceTorch, Main.rand.NextFloat(0.1f, 0.2f), Scale: Main.rand.NextFloat(1f, 1.4f));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(220, 280)).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
                Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(24, 24) - center).SafeNormalize(Vector2.UnitY) * 12;
                int damage = (int)(Projectile.damage * 0.35f);
                if (damage > 26)
                    damage = 26;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, velocity,
                    ModContent.ProjectileType<IcicleFalling>(), damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

    }
}