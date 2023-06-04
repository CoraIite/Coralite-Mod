using Coralite.Helpers;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Coralite.Core;
using Terraria.Audio;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IcicleProj_Hostile : ModProjectile
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

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Ice, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.2f, 0.5f));
                Dust.NewDustPerfect(Projectile.Center, DustID.SnowBlock, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));
            }

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }
    }
}
