using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IcicleFalling_Hostile : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + "Old_IcicleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 100;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.1f;

            Projectile.hostile = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
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
            if (Projectile.timeLeft % 2 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.FrostStaff, -Projectile.velocity * 0.2f, Scale: Main.rand.NextFloat(1f, 1.3f));
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

    }
}
