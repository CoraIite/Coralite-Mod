using Coralite.Core;
using Coralite.Core.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleArrow:ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.velocity.Y += 0.04f;
            if (Projectile.velocity.Y > 16)
                Projectile.velocity.Y = 16;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(140, 220)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(24, 24) - center).SafeNormalize(Vector2.UnitY) * 12;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, velocity,
                    ModContent.ProjectileType<IcicleFalling>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

    }
}