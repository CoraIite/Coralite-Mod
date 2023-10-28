using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IcicleAttack : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + "Old_IcicleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.1f;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;

            Projectile.velocity.Y += 0.04f;
            if (Projectile.velocity.Y > 14f)
                Projectile.velocity.Y = 14f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));
            }

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }
    }
}