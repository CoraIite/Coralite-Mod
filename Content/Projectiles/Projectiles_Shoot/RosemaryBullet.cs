using System;
using Coralite.Content.Particles;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    /// <summary>
    /// 使用ai0控制是否能产生特殊弹幕，为0时能产生花雾弹幕
    /// </summary>
    public class RosemaryBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ArethusaPetal>(), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.05f, 0.15f));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(255,179,251).ToVector3()*0.5f);   //粉色的魔法数字
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Ice_Purple, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                dust.noGravity = true;
            }

            if (Main.myPlayer != Projectile.owner || Projectile.ai[0] != 0)
                return;

            if (hit.Crit)
            {
                SpawnFogProj();
                return;
            }

            if (!target.active)
            {
                SpawnFogProj();
                return;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Ice_Purple, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                dust.noGravity = true;
            }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }

        public void SpawnFogProj()
        {
            int length = Main.rand.Next(160, 200);
            float rotate = Main.rand.NextFloat(6.282f);

            Vector2 dir = rotate.ToRotationVector2();
            Vector2 targetCenter = Projectile.Center + dir * length;
            for (int i = 0; i < 8; i++)
            {
                if (Collision.CanHitLine(Projectile.Center, 1, 1, targetCenter, 1, 1))
                    break;

                rotate += 0.785f;
                dir = rotate.ToRotationVector2();
                targetCenter = Projectile.Center + dir * length;
            }

            float roughlySpeed = length / 12f;
            FlowLine.Spawn(Projectile.Center + dir * 8, dir * roughlySpeed, 2, 12, 0.04f, new Color(95, 120, 233, 100));
            for (int i = -1; i < 4; i += 2)
            {
                FlowLine.Spawn(Projectile.Center + dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * (32 + i * 8), dir * roughlySpeed * 0.5f, 1, 12, Math.Sign(i) * 0.1f, new Color(255, 179, 251, 60));
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), targetCenter, Vector2.Zero,
                ModContent.ProjectileType<RosemaryFog>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
        }
    }
}