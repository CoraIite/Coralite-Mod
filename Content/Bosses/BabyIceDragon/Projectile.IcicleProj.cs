using Coralite.Core;
using InnoVault.GameContent.BaseEntity;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IcicleProj_Hostile : BaseHeldProj
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + "IcicleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;

            Projectile.hostile = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.coldDamage = true;
        }

        public override void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Vector2.UnitY.RotatedBy(i * 0.785f) * 1.5f);
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;

            Projectile.velocity.Y += 0.06f;
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;

            if (Projectile.timeLeft % 2 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.FrostStaff, -Projectile.velocity * 0.2f, Scale: Main.rand.NextFloat(1f, 1.3f));
                dust.noGravity = true;
            }

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Ice, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.2f, 0.5f));
                Dust.NewDustPerfect(Projectile.Center, DustID.SnowBlock, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));
            }

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return base.PreDraw(ref lightColor);
        }
    }
}
