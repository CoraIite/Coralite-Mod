using Coralite.Content.DamageClasses;
using Coralite.Content.Dusts;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineSentinelBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float Timer => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        
        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.scale = 1.4f;
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            SetRotation();
            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() * 0.4f);

            Projectile.ShimmerReflect();

            if (Projectile.timeLeft % 2 == 0 && Main.rand.NextBool())
            {
                Vector2 position = Projectile.Center + Helper.NextVec2Dir(1, 3);
                Vector2 vel = Projectile.DirectionTo(position).RotatedBy(-MathHelper.PiOver2) * Main.rand.NextFloat(1, 2);
                var prt = PRTLoader.NewParticle<CrystallineFlashParticle>(position - vel * 5, vel * 0.75f + Projectile.velocity * 0.25f);
                prt.Scale /= 2f;
            }
        }

        public virtual void SetRotation()
        {
            if (Projectile.localAI[1]==0)
            {
                Projectile.localAI[1] = 1;
                Target = -1;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Target.GetNPCOwner(out NPC target, () => Target = -1))
            {
                Projectile.ChaseGradually(target.Center, 15, 19, 20);
            }
            else
            {
                Timer++;
                if (Timer % 10 == 0)
                {
                    Timer = 0;
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target2))
                    {
                        Target = target2.whoAmI;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.TileReflect(oldVelocity, 0.8f);

            Projectile.ai[2]++;
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), ModContent.DustType<CrystallineDustSmall>(),
                       -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.1f, 0.3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }

            return Projectile.ai[2] > 2;
        }

        public override void OnKill(int timeLeft)
        {
            int prtCount = 3;
            for (int i = 0; i < prtCount; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 3.5f);
                var prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos, vel);
                prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }

            for (int i = 0; i < 6; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16);
                Vector2 vel = Projectile.rotation.ToRotationVector2().RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 3);
                PRTLoader.NewParticle<CrystallineFlashParticle>(position, vel * 0.75f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 6, 1, 6, 1);
            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }

    public class CrystallineSentinelBullet2 : CrystallineSentinelBullet
    {
        public override void SetDefaults()
        {
            Projectile.scale = 1.2f;
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 60 * 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            base.OnTileCollide(oldVelocity);
            if (Projectile.timeLeft < 60 * 2)
            {
                Projectile.timeLeft = 60 * 2;
            }
            return Projectile.ai[2] > 7;
        }

        public override void SetRotation()
        {
            Projectile.rotation += 0.1f + (Projectile.whoAmI % 5) * 0.01f;
        }
    }
}
