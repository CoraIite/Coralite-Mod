using Coralite.Core;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Coralite.Core.Configs;

namespace Coralite.Content.Items.Icicle
{
    /// <summary>
    /// 冰柱坠击
    /// </summary>
    public class IcicleFalling : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + "Old_IcicleProj";

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

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frozen, 180);
        }
    }
}
