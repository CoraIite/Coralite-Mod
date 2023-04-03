using Coralite.Core;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Coralite.Content.Items.IcicleItems
{
    /// <summary>
    /// 冰柱坠击
    /// </summary>
    public class IcicleFalling : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + "Old_IcicleProj";

        public Vector2 TargetCenter
        {
            get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
            set
            {
                Projectile.ai[0] = value.X;
                Projectile.ai[1] = value.Y;
                Projectile.netUpdate = true;
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 25;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.1f;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                TargetCenter += Main.rand.NextVector2CircularEdge(8, 8);
                Projectile.velocity = (TargetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f;
            }

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Vector2.UnitY.RotatedBy(i * 0.785f)*1.5f);
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f,0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frozen, 180);
        }

    }
}
